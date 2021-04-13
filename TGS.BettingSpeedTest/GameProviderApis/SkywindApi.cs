using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class SkywindApi : IGameProviderApi
    {
        private static readonly ISkywindApiClient SkywindApiClient = new SkywindApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            SkywindSettings.ApiEndpoint =
                string.Format(SkywindSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return SkywindSettings.ApiEndpoint;
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
            return PlaceBet(userId, playerData.playerid, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData.playerid, authorizePlayerResponse, count);
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            return PlaceBet(userId, playerData.playerid, authorizePlayerResponse, count);
        }

        private async Task PlaceBet(string userId, Guid playerId, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var externalTransactionId = $"{userId}-Place-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            var roundId = $"{userId}-Round-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            var placeBetResopnse = await SkywindApiClient.PlaceBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, externalTransactionId, 1);

            if (placeBetResopnse == null)
            {
                Logger.Warn($"Player {userId} failed to place bet");
                return;
            }

            Logger.Info($"Player {userId} place bet");

            if (count % 2 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var winBetResopnse = await SkywindApiClient.SettleBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, externalTransactionId, 2);

                if (winBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to settle a bet");
                    return;
                }

                Logger.Info($"Player {userId} settled a bet");
            }
            else
            {
                Logger.Info($"Player {userId} losing a bet");
                var loseBetResopnse = await SkywindApiClient.SettleBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, externalTransactionId, 0);

                if (loseBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to lost a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }
    }
}