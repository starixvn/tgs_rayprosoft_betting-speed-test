using System;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.Operator;

namespace TGS.BettingSpeedTest.GameProviderApis
{
    public interface IGameProviderApi
    {
        string SetupHttpsForEndpoint(bool enableHttps);

        Task InitializeAsync(string userId, string playerToken, Guid? rewardCampaignId);

        Task<GetAccessTokenResponse> GetGameProviderAccessTokenResponseAsync();

        Task PlaceSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId, TokenStore oauthToken,
            AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count);

        Task PlaceMultipleBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count);

        Task PlaceDuplicateSingleBet(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count);

        Task PlaceOppositeBets(string userId, PlayerDataResponse playerData, Guid? rewardCampaignId,
            TokenStore oauthToken, AuthorizePlayerResponse authorizePlayerResponse, bool enableJackpot, int count);
    }
}