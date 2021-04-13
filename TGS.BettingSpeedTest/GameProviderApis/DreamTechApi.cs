using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class DreamTechApi : IGameProviderApi
    {
        private static readonly IDreamTechApiClient DreamTechApiClient = new DreamTechApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            DreamTechSettings.ApiEndpoint =
                string.Format(DreamTechSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return DreamTechSettings.ApiEndpoint;
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return Task.FromResult(0);
        }

        Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            return Task.FromResult(new GetAccessTokenResponse());
        }

        Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData.playercode, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData.playercode, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData.playercode, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        private async Task PlaceBet(string userId, string playerCode, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var placeBetExternalTransactionId = $"{userId}-Place-{DateTime.Now:yyyyMMddHHmmss}-{count}";

            if (count % 2 == 0)
            {
                Logger.Info($"Player {userId} placing and wining a bet");
                var placeAndWinBetResopnse = await DreamTechApiClient.PlaceAndSettleBetAsync(authorizePlayerResponse.authtoken, playerCode, placeBetExternalTransactionId, "1.25");

                if (placeAndWinBetResopnse == null || placeAndWinBetResopnse.Code != "0000")
                {
                    Logger.Warn($"Player {userId} failed to place and win bets");
                    return;
                }

                Logger.Info($"Player {userId} place and win bets");
            }
            else
            {
                Logger.Info($"Player {userId} placing and losing a bet");
                var laceAndLoseBetResponse = await DreamTechApiClient.PlaceAndSettleBetAsync(authorizePlayerResponse.authtoken, playerCode, placeBetExternalTransactionId, "0.00");

                if (laceAndLoseBetResponse == null || laceAndLoseBetResponse.Code != "0000")
                {
                    Logger.Warn($"Player {userId} failed to place and lose bets");
                    return;
                }

                Logger.Info($"Player {userId} place and lost bets");
            }
        }
    }
}