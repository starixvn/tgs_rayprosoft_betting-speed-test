using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class TycheApi : IGameProviderApi
    {
        private static readonly ITycheApiClient TycheApiClient = new TycheApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            TycheSettings.ApiEndpoint = string.Format(TycheSettings.ApiEndpoint, enableHttps ? "https" : "http");
            return TycheSettings.ApiEndpoint;
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

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        private async Task PlaceBet(string userId, Guid playerId, AuthorizePlayerResponse authorizePlayerResponse, int count)
        {
            var placeBetExternalTransactionId = $"{userId}-Place-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            var roundId = $"{userId}-Round-{DateTime.Now:yyyyMMddHHmmss}-{count}";
            var placeBetResopnse = await TycheApiClient.PlaceBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, placeBetExternalTransactionId, 1);

            if (placeBetResopnse == null)
            {
                Logger.Warn($"Player {userId} failed to place bet");
                return;
            }

            Logger.Info($"Player {userId} place bet");

            if (count % 2 == 0)
            {
                Logger.Info($"Player {userId} settling a bet");
                var winBetExternalTransactionId = $"{userId}-Win-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var winBetResopnse = await TycheApiClient.SettleBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, winBetExternalTransactionId, 2);

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
                var loseBetExternalTransactionId = $"{userId}-Lose-{DateTime.Now:yyyyMMddHHmmss}-{count}";
                var loseBetResopnse = await TycheApiClient.SettleBetAsync(authorizePlayerResponse.authtoken, playerId, roundId, loseBetExternalTransactionId, 0);

                if (loseBetResopnse == null)
                {
                    Logger.Warn($"Player {userId} failed to lost a bet");
                    return;
                }

                Logger.Info($"Player {userId} lost a bet");
            }
        }
    }
}