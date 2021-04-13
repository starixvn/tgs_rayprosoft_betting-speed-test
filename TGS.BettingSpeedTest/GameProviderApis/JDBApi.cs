using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.AWC;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public sealed class JDBApi : IGameProviderApi
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IAWCApiClient ApiClient = new AWCApiClient();

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            JDBSettings.ApiEndpoint =
                string.Format(JDBSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return JDBSettings.ApiEndpoint;
        }

        Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            return Task.FromResult(new GetAccessTokenResponse());
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return BetsAndSettleAsync(userId, playerData, authorizePlayerResponse, count);
        }

        async Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            var transactionId = $"{userId}-bet-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            await BetsAndSettleAsync(userId, playerData, authorizePlayerResponse, count, transactionId);
            await BetsAndSettleAsync(userId, playerData, authorizePlayerResponse, count, transactionId);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return BetsAndSettleAsync(userId, playerData, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        private Task BetsAndSettleAsync(string userId, PlayerDataResponse playerData, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var transactionId = $"{userId}-bet-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            return BetsAndSettleAsync(userId, playerData, authorizePlayerResponse, count, transactionId);
        }

        private async Task BetsAndSettleAsync(string userId, PlayerDataResponse playerData, AuthorizePlayerResponse authorizePlayerResponse, int count, string externalTransactionId)
        {
            var isWin = count % 2 == 0;
            
            Logger.Info($"Player {userId} placing and {(isWin ? "winning" : "losing")} a bet");

            var transactions = new[]
            {
                new Transaction
                {
                    PlayerCode = playerData.playercode,
                    ExternalTransactionId = externalTransactionId,
                    ExternalRoundId = externalTransactionId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameId = JDBSettings.TestGameId,
                    Currency = "CNY",
                    PlatformName = JDBSettings.AWCPlatform,
                    GameType = JDBSettings.GameType,
                    BetAmount = 1,
                    WinAmount = isWin ? 10 : 0,
                    BetTime = DateTimeOffset.UtcNow.AddSeconds(-1),
                    UpdateTime = DateTimeOffset.UtcNow
                }
            };

            var response = await ApiClient.RequestAsync(JDBSettings.ApiEndpoint, JDBSettings.Key, authorizePlayerResponse.authtoken, transactions, "betNSettle");

            if (response == null || response.Status != "0000")
            {
                Logger.Warn($"Player {userId} failed to place and {(isWin ? "win" : "lose")} bets");
                return;
            }

            Logger.Info($"Player {userId} place and {(isWin ? "win" : "lose")} bets");
        }
    }
}
