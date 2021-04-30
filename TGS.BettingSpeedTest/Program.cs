using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.GameProviderApis;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.GameProvider;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Messages.SunbetCredit;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest
{
    internal static class Program
    {
        private static readonly IOperatorApiClient OperatorApiClient = new OperatorApiClient();
        private static readonly ISystemApiClient SystemApiClient = new SystemApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();

        private static readonly TokenStore GpTokenStore = new TokenStore(
            response => response is BetResponse betResponse && betResponse.err == 10,
            async gpApi =>
            {
                var res = await gpApi.GetGameProviderAccessTokenResponseAsync();
                return res.access_token;
            }
        );

        private static readonly IDictionary<string, IGameProviderApi> GameProviderApiList =
            new Dictionary<string, IGameProviderApi>
            {
                {"AG", new AsiaGamingApi()}
            };

        private static readonly
            IDictionary<string, Func<IGameProviderApi, string, PlayerDataResponse, Guid?, TokenStore,
                AuthorizePlayerResponse, bool, int, Task>> ScenarioList =
                new Dictionary<string, Func<IGameProviderApi, string, PlayerDataResponse, Guid?, TokenStore,
                    AuthorizePlayerResponse, bool, int, Task>>
                {
                    {
                        "PlaceAndSettle", (gpApi, userId, playerDataResponse, rewardCampaignId, oauthToken,
                                authorizePlayerResponse, enableJackpot, count) =>
                            gpApi.PlaceSingleBet(userId, playerDataResponse, rewardCampaignId, GpTokenStore,
                                authorizePlayerResponse, enableJackpot, count)
                    },
                    {
                        "MultipleBets", (gpApi, userId, playerDataResponse, rewardCampaignId, oauthToken,
                            authorizePlayerResponse, enableJackpot, count) =>
                        {
                            if (count % 3 == 0)
                                return gpApi.PlaceMultipleBets(userId, playerDataResponse, rewardCampaignId,
                                    GpTokenStore, authorizePlayerResponse, enableJackpot, count);

                            return gpApi.PlaceSingleBet(userId, playerDataResponse, rewardCampaignId, GpTokenStore,
                                authorizePlayerResponse, enableJackpot, count);
                        }
                    },
                    {
                        "PlaceAndSettleWithDuplicates", (gpApi, userId, playerDataResponse, rewardCampaignId,
                                oauthToken, authorizePlayerResponse, enableJackpot, count) =>
                            gpApi.PlaceDuplicateSingleBet(userId, playerDataResponse, rewardCampaignId, GpTokenStore,
                                authorizePlayerResponse, enableJackpot, count)
                    },
                    {
                        "PlaceOppositeBetsAndSettle", (gpApi, userId, playerDataResponse, rewardCampaignId, oauthToken,
                                authorizePlayerResponse, enableJackpot, count) =>
                            gpApi.PlaceOppositeBets(userId, playerDataResponse, rewardCampaignId, GpTokenStore,
                                authorizePlayerResponse, enableJackpot, count)
                    }
                };

        private static readonly AutoResetEvent WaitEvent = new AutoResetEvent(false);

        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Cts.Cancel();
                WaitEvent.Set();
                Console.WriteLine("Process exited by killing process!");
            };

            Console.CancelKeyPress += (s, e) =>
            {
                Cts.Cancel();
                WaitEvent.Set();
                Console.WriteLine("Process exited by ctrl + c!");
            };

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunTest(opts).GetAwaiter().GetResult())
                .WithNotParsed(HandleParseError);

            WaitEvent.WaitOne();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var err in errs)
            {
                Console.WriteLine(err.Tag);
            }
        }

        private static async Task RunTest(Options options)
        {
            var playCycle = options.PlayCycleOfPercentage;
            var millisecondsDelay = options.MillisecondsDelay;
            var percentageOfPlay = options.PercentageOfPlay;
            var brandCredential = GetCredentialAndLobbyPath(options.BrandCode);
            if (brandCredential == null)
            {
                Logger.Warn("Unable to get operator credential");
                return;
            }

            var isSunbetCredit = options.BrandCode == "SunbetCredit";
            var isProxyWallet = options.BrandCode == "PROXYBRAND1";
            var numberOfPlayers = options.NumberOfPlayers;
            var enableJackpot = options.EnableJackpot;

            ServicePointManager.DefaultConnectionLimit = numberOfPlayers * 3;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            if (!GameProviderApiList.TryGetValue(options.GameProviderCode, out var gpApi))
            {
                Logger.Warn($"Failed to get game provider {options.GameProviderCode}");
                return;
            }

            if (!ScenarioList.TryGetValue(options.Scenario, out var scenario))
            {
                Logger.Warn($"Failed to get scenario {options.Scenario}");
                return;
            }

            var gpEndpoint = gpApi.SetupHttpsForEndpoint(options.EnableHttps);
            var operatorAccessTokenResponse = isSunbetCredit
                ? new GetAccessTokenResponse()
                : await GetOperatorAccessTokenResponseAsync(brandCredential);
            await GpTokenStore.GetTokenAsync(gpApi);

            Guid? rewardCampaignId = null;
            if (options.GameProviderCode == "Reward")
            {
                var response =
                    await SystemApiClient.CreateTestRewardCampaignAsync(options.RewardCampaignRestrictionType,
                        options.ChanceToWin);
                rewardCampaignId = response.Id;
            }

            Console.WriteLine($"Executing {options.Scenario} scenario");
            Console.WriteLine($"Executing {numberOfPlayers} players testing");
            Console.WriteLine($"Enable Jackpot testing {enableJackpot}");
            Console.WriteLine($"Version {typeof(Program).Assembly.GetName().Version}");
            Console.WriteLine($"Api: {gpEndpoint}");
            Console.WriteLine("Press ctrl+c to stop service........");

            Logger.Info(Environment.NewLine);

            var totalPlayers = Enumerable.Range(0, numberOfPlayers).ToArray();

            // initialize players in batches to avoid large thread initialization at the same time
            // which may lead to unexpected timeout
            const int batchSize = 50;
            var numberOfBatches = (int) Math.Ceiling((double) numberOfPlayers / batchSize);

            for (var currentBatch = 0; currentBatch < numberOfBatches; currentBatch++)
            {
                var currentBatchSize = totalPlayers.Skip(currentBatch * batchSize).Take(batchSize);

                var tasks = Enumerable.Range(currentBatch * batchSize, currentBatchSize.ToArray().Length).Select(i =>
                    Task.Factory.StartNew(async () =>
                    {
                        var userId = $"{Environment.MachineName}-{i:D3}";
                        Console.WriteLine(
                            $"{userId} starts at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} :{Thread.CurrentThread.ManagedThreadId}");

                        Logger.Info($"Authorizing player {userId}");
                        var authorizePlayerResponse = await AuthorizePlayerAsync(i, userId,
                            operatorAccessTokenResponse.access_token, isSunbetCredit);
                        if (authorizePlayerResponse == null)
                        {
                            Logger.Warn($"Failed to authorize player {userId}");
                            return;
                        }

                        if (isSunbetCredit) userId = authorizePlayerResponse.userId;

                        Logger.Info($"Authorized player {userId}");

                        await gpApi.InitializeAsync(userId, authorizePlayerResponse.authtoken, rewardCampaignId);

                        Logger.Info($"Getting player {userId} info");
                        var playerInfoResponse =
                            await OperatorApiClient.PlayerInfoAsync(authorizePlayerResponse.authtoken);

                        if (playerInfoResponse == null)
                        {
                            Logger.Warn($"Failed to get player {userId} info");
                            return;
                        }

                        Logger.Info($"Got player {userId} info");

                        if (!isProxyWallet)
                        {
                            var creditAccessToken = isSunbetCredit
                                ? authorizePlayerResponse.sunbetcredittoken
                                : operatorAccessTokenResponse.access_token;
                            var credited = await CreditPlayerAsync(userId, playerInfoResponse.playerid,
                                creditAccessToken, isSunbetCredit);
                            if (!credited) return;
                        }

                        var j = 1;
                        while (!Cts.IsCancellationRequested)
                        {
                            if (j % playCycle == 0 && !CanPlay(percentageOfPlay))
                            {
                                j++;
                                await Task.Delay(millisecondsDelay);
                                continue;
                            }

                            try
                            {
                                Logger.Info($"Player {userId} starts scenario");

                                await scenario(gpApi, userId, playerInfoResponse, rewardCampaignId, GpTokenStore,
                                    authorizePlayerResponse, enableJackpot, j);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                            finally
                            {
                                j++;
                                await Task.Delay(millisecondsDelay);
                            }
                        }
                    }, TaskCreationOptions.LongRunning));

                await Task.WhenAll(tasks);

                // sleep in between each batch
                Thread.Sleep(5000);
            }
        }

        private static bool CanPlay(byte percentageOfPlay)
        {
            if (percentageOfPlay == 100) return true;
            if (percentageOfPlay > 100) return true;
            if (percentageOfPlay == 0) return false;

            var rnd = new Random();
            var rndNumber = rnd.Next(1, 100);

            if (percentageOfPlay > 50) return rndNumber > 100 - percentageOfPlay;
            return rndNumber <= percentageOfPlay;
        }

        private static async Task<AuthorizePlayerResponse> AuthorizePlayerAsync(int playerInex, string userId,
            string accessToken, bool isSunbetCredit)
        {
            if (isSunbetCredit)
            {
                var authed = await OperatorApiClient.SunbetCreditAuthManyAsync(playerInex + 1);
                if (authed.return_code != ApiResponseCode.Succeeded)
                {
                    Logger.Warn("Failed to authorize sunbet credit");
                    return null;
                }

                var response = authed.data.FirstOrDefault();
                if (response == null)
                {
                    return null;
                }

                var player = await
                    OperatorApiClient.SunbetCreditTokenAuthedAsync(response.SessionId, response.AcctId,
                        response.SiteId);
                if (player.err != ApiResponseCode.Succeeded)
                {
                    Logger.Warn("Failed to get sunbet credit token");
                    return null;
                }

                return new AuthorizePlayerResponse
                {
                    authtoken = player.PlayerToken,
                    sunbetcredittoken = player.SunbetCreditToken,
                    userId = response.AcctId
                };
            }

            return await OperatorApiClient.AuthorizePlayerAsync(accessToken, userId);
        }

        private static async Task<bool> CreditPlayerAsync(string userId, Guid ugsUserId, string accessToken,
            bool isSunbetCredit)
        {
            Logger.Info($"Crediting player {userId}");

            if (isSunbetCredit)
            {
                var playerInfo = await
                    OperatorApiClient.SunbetCreditGetPlayerInfoAsync(userId);

                if (playerInfo.return_code != ApiResponseCode.Succeeded)
                {
                    Logger.Warn($"Failed to get player {userId} information from sunbet credit");
                    return false;
                }

                if (playerInfo.data.wallet_balance > 0)
                {
                    var debitResult = await
                        OperatorApiClient.SunbetCreditWalletDebitAsync(accessToken, ugsUserId);

                    if (debitResult.err != ApiResponseCode.Succeeded)
                    {
                        Logger.Warn($"Failed to debit. Player: {userId}");
                        return false;
                    }
                }

                if (playerInfo.data.main_balance < OperatorSettings.Wallet.CreditAmount)
                {
                    Logger.Warn($"Insufficient fund of sunbet credit. Player: {userId}");
                    return false;
                }

                var credited = await
                    OperatorApiClient.SunbetCreditWalletCreditAsync(accessToken, ugsUserId);

                if (credited.err != ApiResponseCode.Succeeded)
                {
                    Logger.Warn(
                        $"Failed to withdraw sunbet credit wallet. Player: {playerInfo.data.acct_id}. Error: {credited.errdesc}");
                    return false;
                }
            }
            else
            {
                var creditExternalTransactionId = $"{userId}-Credit-{DateTime.Now:yyyyMMddHHmmss}";
                var creditResponse = await OperatorApiClient.CreditAsync(accessToken, userId,
                    creditExternalTransactionId);

                if (creditResponse == null)
                {
                    Logger.Warn($"Failed to credit player {userId}");
                    return false;
                }
            }

            Logger.Info($"Credited {userId}");
            return true;
        }

        private static async Task<GetAccessTokenResponse> GetOperatorAccessTokenResponseAsync(
            CredentialAndLobbyPath credential)
        {
            Logger.Info("Getting operator access token");
            var getAccessTokenResponse = await OperatorApiClient.GetAccessTokenAsync(credential);

            if (getAccessTokenResponse == null)
            {
                throw new InvalidOperationException("Failed to get operator access token");
            }

            return getAccessTokenResponse;
        }

        private static CredentialAndLobbyPath GetCredentialAndLobbyPath(string brandCode)
        {
            if (OperatorSettings.Credentials.TryGetValue(brandCode, out var credential))
            {
                return credential;
            }

            throw new InvalidOperationException("Brand credential not found.");
        }
    }
}