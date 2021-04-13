using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public sealed class OneTouchApi : IGameProviderApi
    {
        private static readonly IOneTouchApiClient ApiClient = new OneTouchApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            OneTouchSettings.ApiEndpoint =
                string.Format(OneTouchSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return OneTouchSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        async Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            Logger.Info("Getting game provider access token");
            var getAccessTokenResponse = await ApiClient.GetAccessTokenAsync();

            if (getAccessTokenResponse == null)
            {
                throw new InvalidOperationException("Failed to get game provider access token");
            }

            return getAccessTokenResponse;
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, oauthToken, authorizePlayerResponse, count);
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

            await SettleBet(userId, oauthToken, authorizePlayerResponse, count, placeBetExternalTransactionId, timestamp);
            await SettleBet(userId, oauthToken, authorizePlayerResponse, count, placeBetExternalTransactionId, timestamp);
        }

        private async Task PlaceBet(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";
            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);
            var betPlaced = await PlaceBet(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;

            await SettleBet(userId, oauthToken, authorizePlayerResponse, count, placeBetExternalTransactionId, timestamp);
        }

        private async Task SettleBet(string userId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, int count,
            string placeBetExternalTransactionId, string timestamp)
        {
            if (count % 5 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var settleBetExternalTransactionId = GetExternalTransactionId(userId, "Settle", timestamp, count);
                var settleBetResponse = await ApiClient.SettleBetAsync(
                    oauthToken.Token, authorizePlayerResponse.authtoken,
                    settleBetExternalTransactionId, placeBetExternalTransactionId);

                if (settleBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                var loseBetExternalTransactionId = GetExternalTransactionId(userId, "Lose", timestamp, count);
                var loseBetResponse = await ApiClient.LoseBetAsync(
                    oauthToken.Token, authorizePlayerResponse.authtoken,
                    loseBetExternalTransactionId, placeBetExternalTransactionId);

                if (loseBetResponse == null)
                {
                    Logger.Warn($"Player {userId} failed to lose a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        private async Task<bool> PlaceBet(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            string placeBetExternalTransactionId)
        {
            var placeBetResponse = await ApiClient.PlaceBetAsync(
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
            return $"{userId}-{action}-{timestamp}-{count}";
        }
    }
}