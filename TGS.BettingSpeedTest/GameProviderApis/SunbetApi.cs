using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Extensions;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.GameProvider;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public sealed class SunbetApi : IGameProviderApi
    {
        private static readonly ISunbetApiClient SunbetApiClient = new SunbetApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            SunbetSettings.ApiEndpoint =
                string.Format(SunbetSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return SunbetSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        async Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            Logger.Info("Getting game provider access token");
            var getAccessTokenResponse = await SunbetApiClient.GetAccessTokenAsync();

            if (getAccessTokenResponse == null)
            {
                throw new InvalidOperationException("Failed to get game provider access token");
            }

            return getAccessTokenResponse;
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData, oauthToken, authorizePlayerResponse, count);
        }

        async Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";
            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);
            var settleBetExternalTransactionId = GetExternalTransactionId(userId, "Settle", timestamp, count);

            const string betTypeBanker = "baBank";
            const string betTypePlayer = "baDeal";

            var placeBetTransactions = new []
            {
                new PlaceBetRequest.TransactionRequest
                {
                    amt = 1,
                    cur = OperatorSettings.Player.Currency,
                    txid = $"{placeBetExternalTransactionId}_{betTypeBanker}",
                    roundid = placeBetExternalTransactionId,
                    betrecid = $"{placeBetExternalTransactionId}_{betTypeBanker}",
                    gameid = SunbetSettings.TestGameId,
                    timestamp = DateTimeOffset.Now.ToIso8601String(),
                    desc = betTypeBanker
                },
                new PlaceBetRequest.TransactionRequest
                {
                    amt = 1,
                    cur = OperatorSettings.Player.Currency,
                    txid = $"{placeBetExternalTransactionId}_{betTypePlayer}",
                    roundid = placeBetExternalTransactionId,
                    betrecid = $"{placeBetExternalTransactionId}_{betTypePlayer}",
                    gameid = SunbetSettings.TestGameId,
                    timestamp = DateTimeOffset.Now.ToIso8601String(),
                    desc = betTypePlayer
                }
            };

            var settleTransactions = new []
            {
                new BatchRequest.BatchRequestTransaction
                {
                    amt = 10,
                    cur = OperatorSettings.Player.Currency,
                    txid = $"{settleBetExternalTransactionId}_{betTypeBanker}",
                    txtype = TransactionTypes.Win,
                    txrefid = $"{placeBetExternalTransactionId}_{betTypeBanker}",
                    roundid = placeBetExternalTransactionId,
                    betrecid = $"{placeBetExternalTransactionId}_{betTypeBanker}",
                    userid = playerData.playerid,
                    gameid = SunbetSettings.TestGameId,
                    desc = betTypeBanker
                },
                new BatchRequest.BatchRequestTransaction
                {
                    amt = 10,
                    cur = OperatorSettings.Player.Currency,
                    txid = $"{settleBetExternalTransactionId}_{betTypePlayer}",
                    txtype = TransactionTypes.Loss,
                    txrefid = $"{placeBetExternalTransactionId}_{betTypePlayer}",
                    roundid = placeBetExternalTransactionId,
                    betrecid = $"{settleBetExternalTransactionId}_{betTypePlayer}",
                    userid = playerData.playerid,
                    gameid = SunbetSettings.TestGameId,
                    desc = betTypePlayer
                },
            };

            await PlaceBetsAsync(userId, oauthToken, authorizePlayerResponse, placeBetTransactions);
            await SettleBetsAsync(userId, oauthToken, settleTransactions);
        }

        async Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";
            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);
            var betPlaced = await PlaceBet(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;
            betPlaced = await PlaceBet(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;

            if (count % 5 == 0)
            {
                await WinBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp, "WinBetOriginal");
                await WinBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp, "WinBetDuplicate");
            }
            else
            {
                await LoseBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp, "LoseBetOriginal");
                await LoseBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp, "LoseBetDuplicate");
            }
        }

        private async Task PlaceBet(string userId, PlayerDataResponse playerData, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";
            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);
            var betPlaced = await PlaceBet(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;

            if (count % 5 == 0)
            {
                await WinBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp);
            }
            else
            {
                await LoseBetAsync(userId, playerData.playerid, oauthToken, count, placeBetExternalTransactionId, timestamp);
            }
        }

        private async Task LoseBetAsync(string userId, Guid playerId, TokenStore oauthToken, int count,
            string placeBetExternalTransactionId, string timestamp, string description = "")
        {
            Logger.Info($"Player {userId} losing a bet");
            var loseBetExternalTransactionId = GetExternalTransactionId(userId, "Lose", timestamp, count);
            var loseBetResponse = await SunbetApiClient.LoseBetAsync(
                oauthToken.Token, playerId,
                loseBetExternalTransactionId, placeBetExternalTransactionId, description);

            if (loseBetResponse == null)
            {
                Logger.Warn($"Player {userId} failed to lose a bet");
                return;
            }

            Logger.Info($"Player {userId} lost a bet");
        }

        private async Task WinBetAsync(string userId, Guid playerId, TokenStore oauthToken, int count,
            string placeBetExternalTransactionId, string timestamp, string description = "")
        {
            Logger.Info($"Player {userId} settling a bet");
            var settleBetExternalTransactionId = GetExternalTransactionId(userId, "Win", timestamp, count);
            var settleBetResponse = await SunbetApiClient.SettleBetAsync(
                oauthToken.Token, playerId,
                settleBetExternalTransactionId, placeBetExternalTransactionId, description);

            if (settleBetResponse == null)
            {
                Logger.Warn($"Player {userId} failed to settle a bet");
                return;
            }

            Logger.Info($"Player {userId} settled a bet");
        }

        private async Task<bool> PlaceBet(string userId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, string placeBetExternalTransactionId)
        {
            var placeBetResponse = await SunbetApiClient.PlaceBetAsync(
                oauthToken.Token, authorizePlayerResponse.authtoken,
                placeBetExternalTransactionId);

            await oauthToken.RefreshTokenIfNeeded(placeBetResponse, this);
            if (placeBetResponse == null || placeBetResponse.err != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return false;
            }

            Logger.Info($"Player {userId} placed a bet");
            return true;
        }

        private async Task<bool> PlaceBetsAsync(string userId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, PlaceBetRequest.TransactionRequest[] transactionRequests)
        {
            var placeBetResponse =
                await SunbetApiClient.PlaceBetsAsync(oauthToken.Token, authorizePlayerResponse.authtoken, transactionRequests);

            await oauthToken.RefreshTokenIfNeeded(placeBetResponse, this);
            if (placeBetResponse == null || placeBetResponse.err != 0)
            {
                Logger.Warn($"Player {userId} failed to place a bet");
                await Task.Delay(1000);
                return false;
            }

            Logger.Info($"Player {userId} placed a bet");
            return true;
        }

        private static async Task SettleBetsAsync(string userId, TokenStore oauthToken,
            BatchRequest.BatchRequestTransaction[] batchRequestTransactions)
        {
            Logger.Info($"Player {userId} settling a bet");
            var settleBetResponse =
                await SunbetApiClient.SettleBetsAsync(oauthToken.Token, batchRequestTransactions);

            if (settleBetResponse == null)
            {
                Logger.Warn($"Player {userId} failed to settle a bet");
                return;
            }

            Logger.Info($"Player {userId} settled a bet");
        }

        private string GetExternalTransactionId(string userId, string action, string timestamp, int count)
        {
            return $"{userId}-{action}-{timestamp}-{count}";
        }
    }
}
