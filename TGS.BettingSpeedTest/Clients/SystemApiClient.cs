using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.System;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface ISystemApiClient
    {
        Task<CreateRewardCampaignResponse> CreateTestRewardCampaignAsync(string restrictionType, string chanceToWin);
        Task<string> JoinRewardCampaignAsync(JoinRewardCampaignRequest request);
    }

    internal class SystemApiClient : BaseApiClient, ISystemApiClient
    {
        Task<CreateRewardCampaignResponse> ISystemApiClient.CreateTestRewardCampaignAsync(string restrictionType, string chanceToWin)
        {
            var url = SystemApiSettings.ApiEndpoint + SystemApiSettings.ApiPath.CreateRewardCampaign;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("RestrictionType", restrictionType),
                    new KeyValuePair<string, string>("ChanceToWin", chanceToWin)
                })
            };

            return SendAsync<CreateRewardCampaignResponse>(request);
        }

        Task<string> ISystemApiClient.JoinRewardCampaignAsync(JoinRewardCampaignRequest request)
        {
            var url = SystemApiSettings.ApiEndpoint + SystemApiSettings.ApiPath.JoinRewardCampaign;

            var joinRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("RewardCampaignId", request.RewardCampaignId.ToString()),
                    new KeyValuePair<string, string>("PlayerToken", request.PlayerToken) })
            };

            return SendAsync<string>(joinRequest);
        }
    }
}