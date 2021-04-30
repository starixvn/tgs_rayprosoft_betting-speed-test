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
using TGS.BettingSpeedTest.Messages.Operator;

namespace TGS.BettingSpeedTest
{
    internal static class Program
    {
        private static readonly IOperatorApiClient OperatorApiClient = new OperatorApiClient();
        private static readonly IGlobalApiClient GlobalApiClient = new GlobalApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();

        private static readonly IDictionary<string, IGameProviderApi> GameProviderApiList =
            new Dictionary<string, IGameProviderApi>
            {
                {"AG", new AsiaGamingApi()}
            };

        private static readonly
            IDictionary<string, Func<IGameProviderApi, string, PlayerDataResponse, Guid?,
                AuthorizePlayerResponse, bool, int, Task>> ScenarioList =
                new Dictionary<string, Func<IGameProviderApi, string, PlayerDataResponse, Guid?,
                    AuthorizePlayerResponse, bool, int, Task>>();

        private static readonly AutoResetEvent WaitEvent = new AutoResetEvent(false);

        internal static async Task Main(string[] args)
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
                .WithParsed(async opts => await RunTest(opts).ConfigureAwait(false))
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


            var numberOfPlayers = options.NumberOfPlayers;
            //var enableJackpot = options.EnableJackpot;

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


            Console.WriteLine($"Executing {options.Scenario} scenario");
            Console.WriteLine($"Executing {numberOfPlayers} players testing");
            //Console.WriteLine($"Enable Jackpot testing {enableJackpot}");
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

                        AuthorizePlayerResponse authorizePlayerResponse = null;
                        if (authorizePlayerResponse == null)
                        {
                            Logger.Warn($"Failed to authorize player {userId}");
                            return;
                        }

                        Logger.Info($"Authorized player {userId}");

                        await gpApi.InitializeAsync(userId, authorizePlayerResponse.authtoken, null);

                        Logger.Info($"Getting player {userId} info");
                        var playerInfoResponse =
                            await OperatorApiClient.PlayerInfoAsync(authorizePlayerResponse.authtoken);

                        if (playerInfoResponse == null)
                        {
                            Logger.Warn($"Failed to get player {userId} info");
                            return;
                        }

                        Logger.Info($"Got player {userId} info");

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

                                await scenario(gpApi, userId, playerInfoResponse, null,
                                    authorizePlayerResponse, false, j);
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
            string accessToken, bool isSunbetCredit) =>
            await OperatorApiClient.AuthorizePlayerAsync(accessToken, userId);

        private static async Task<bool> CreditPlayerAsync(string userId, Guid ugsUserId, string accessToken,
            bool isSunbetCredit)
        {
            Logger.Info($"Crediting player {userId}");

            var creditExternalTransactionId = $"{userId}-Credit-{DateTime.Now:yyyyMMddHHmmss}";
            var creditResponse = await OperatorApiClient.CreditAsync(accessToken, userId,
                creditExternalTransactionId);

            if (creditResponse == null)
            {
                Logger.Warn($"Failed to credit player {userId}");
                return false;
            }

            Logger.Info($"Credited {userId}");
            return true;
        }

        //private static CredentialAndLobbyPath GetCredentialAndLobbyPath(string brandCode)
        //{
        //    if (OperatorSettings.Credentials.TryGetValue(brandCode, out var credential))
        //    {
        //        return credential;
        //    }

        //    throw new InvalidOperationException("Brand credential not found.");
        //}
    }
}