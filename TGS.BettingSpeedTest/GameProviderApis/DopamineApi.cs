using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class DopamineApi : IGameProviderApi
    {
        private static readonly IDopamineApiClient DopamineApiClient = new DopamineApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            DopamineSettings.ApiEndpoint =
                string.Format(DopamineSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return DopamineSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        async Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            Logger.Info("Getting game provider access token");
            var getAccessTokenResponse = await DopamineApiClient.GetAccessTokenAsync();

            if (getAccessTokenResponse == null)
            {
                throw new InvalidOperationException("Failed to get game provider access token");
            }

            return getAccessTokenResponse;
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count);
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

            await SettleBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count, placeBetExternalTransactionId, timestamp);
            await SettleBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count, placeBetExternalTransactionId, timestamp);
        }

        private async Task PlaceBet(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";
            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);
            var betPlaced = await PlaceBet(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;

            await SettleBet(userId, oauthToken, authorizePlayerResponse, enableJackpot, count, placeBetExternalTransactionId, timestamp);
        }

        private async Task SettleBet(string userId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count,
            string placeBetExternalTransactionId, string timestamp)
        {
            const string win = "win", winning = "winning", won = "won";
            const string lose = "lose", losing = "losing", lost = "lost";

            var shouldWin = count % 5 == 0;

            Logger.Info($"Player {userId} {(shouldWin ? winning : losing)} a bet");

            var betExternalTransactionId = GetExternalTransactionId(userId, shouldWin ? win : lose, timestamp, count);
            var settleBetResponse = await DopamineApiClient.SettleBetAsync(
                oauthToken.Token, authorizePlayerResponse.authtoken,
                betExternalTransactionId, placeBetExternalTransactionId,
                shouldWin ? TransactionTypes.Win : TransactionTypes.Loss, enableJackpot);

            if (settleBetResponse == null)
            {
                Logger.Warn($"Player {userId} failed to {(shouldWin ? win : lose)} a bet");
                return;
            }

            Logger.Info($"Player {userId} {(shouldWin ? won : lost)} a bet");
        }

        private async Task<bool> PlaceBet(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            string placeBetExternalTransactionId)
        {
            Logger.Info($"Player {userId} placing a bet");

            var placeBetResponse = await DopamineApiClient.PlaceBetAsync(
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

        private string GetExternalTransactionId(string userId, string action, string timestamp, int count)
        {
            return $"{userId}-{action}-{timestamp}-{count}".ToUpper();
        }
    }
}