using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class CardGames365Api : IGameProviderApi
    {
        private static readonly ICardGames365ApiClient CardGames365ApiClient = new CardGames365ApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            CardGames365Settings.ApiEndpoint =
                string.Format(CardGames365Settings.ApiEndpoint, enableHttps ? "https" : "http");
            return CardGames365Settings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        async Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            Logger.Info("Getting game provider access token");
            var getAccessTokenResponse = await CardGames365ApiClient.GetAccessTokenAsync();

            if (getAccessTokenResponse == null)
            {
                throw new InvalidOperationException("Failed to get game provider access token");
            }

            return getAccessTokenResponse;
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBetAndSettleBetAsync(userId, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBetAndSettleBetAsync(userId, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBetAndSettleBetAsync(userId, oauthToken, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBetAndSettleBetAsync(userId, oauthToken, authorizePlayerResponse, count);
        }

        private async Task PlaceBetAndSettleBetAsync(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var timestamp = $"{DateTime.Now:yyMMddHHmmss}";

            var placeBetExternalTransactionId = GetExternalTransactionId(userId, "Place", timestamp, count);

            await LockTransactionAsync(userId, oauthToken, authorizePlayerResponse, count, placeBetExternalTransactionId, timestamp);

            var betPlaced = await PlaceBetAsync(userId, oauthToken, authorizePlayerResponse, placeBetExternalTransactionId);
            if (!betPlaced) return;

            await SettleBetAsync(userId, oauthToken, authorizePlayerResponse, count, placeBetExternalTransactionId, timestamp);
        }

        private async Task<bool> LockTransactionAsync(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            int count, string placeBetExternalTransactionId, string timestamp)
        {
            var lockExternalTransactionId = GetExternalTransactionId(userId, "Lock", timestamp, count);

            var lockResponse = await CardGames365ApiClient.LockTransactionAsync(
                oauthToken.Token, authorizePlayerResponse.authtoken,
                lockExternalTransactionId, placeBetExternalTransactionId);

            await oauthToken.RefreshTokenIfNeeded(lockResponse, this);
            if (lockResponse == null || lockResponse.err != 0)
            {
                Logger.Warn($"Player {userId} failed to lock amount");
                await Task.Delay(1000);
                return false;
            }

            Logger.Info($"Player {userId} lock amount");
            return true;
        }

        private async Task<bool> PlaceBetAsync(string userId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse,
            string placeBetExternalTransactionId)
        {
            var placeBetResponse = await CardGames365ApiClient.PlaceBetAsync(
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

        private async Task SettleBetAsync(string userId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, int count,
            string placeBetExternalTransactionId, string timestamp)
        {
            int amount = 10;

            if (count % 5 != 0)
            {
                amount = 0;
            }

            Logger.Info($"Player {userId} settling a bet");
            var settleBetExternalTransactionId = GetExternalTransactionId(userId, "Settle", timestamp, count);
            var settleBetResponse = await CardGames365ApiClient.SettleBetAsync(
                oauthToken.Token, authorizePlayerResponse.authtoken,
                settleBetExternalTransactionId, placeBetExternalTransactionId,
                amount, amount == 0);

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