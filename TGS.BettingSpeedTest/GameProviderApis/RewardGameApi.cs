using System;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;
using TGS.BettingSpeedTest.Messages.System;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public class RewardGameApi : IGameProviderApi
    {
        private static readonly ISystemApiClient SystemApiClient = new SystemApiClient();
        private static readonly IRewardGameApiClient ApiClient = new RewardGameApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        Task<GetAccessTokenResponse> IGameProviderApi.GetGameProviderAccessTokenResponseAsync()
        {
            return Task.FromResult(new GetAccessTokenResponse());
        }

        Task IGameProviderApi.InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId)
        {
            return SystemApiClient.JoinRewardCampaignAsync(
                new JoinRewardCampaignRequest
                {
                    PlayerToken = playerToken,
                    RewardCampaignId = rewardCampaignId ?? Guid.Empty
                });
        }

        Task IGameProviderApi.PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        Task IGameProviderApi.PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        Task IGameProviderApi.PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData,
            Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            throw new NotImplementedException();
        }

        async Task IGameProviderApi.PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count)
        {
            if (!rewardCampaignId.HasValue)
            {
                throw new ArgumentNullException("reward campaign id is null");
            }
            var response = await ApiClient.DrawAsync(new Messages.GameProvider.RewardCampaignGameRequest { authtoken = authorizePlayerResponse.authtoken, campaignid = rewardCampaignId.Value });
        }

        string IGameProviderApi.SetupHttpsForEndpoint(bool enableHttps)
        {
            RewardGameSetting.ApiEndpoint =
               string.Format(RewardGameSetting.ApiEndpoint, enableHttps ? "https" : "http");
            return RewardGameSetting.ApiEndpoint;
        }
    }
}