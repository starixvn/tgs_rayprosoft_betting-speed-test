using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.GameProvider;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IRewardGameApiClient
    {
        Task<RewardCampaignGameResponse> DrawAsync(RewardCampaignGameRequest request);
    }

    internal class RewardGameApiClient : BaseApiClient, IRewardGameApiClient
    {
        Task<RewardCampaignGameResponse> IRewardGameApiClient.DrawAsync(RewardCampaignGameRequest rewardRequest)
        {
            var url = RewardGameSetting.ApiEndpoint + RewardGameSetting.ApiPath.Draw;
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<RewardCampaignGameRequest>(rewardRequest, new JsonMediaTypeFormatter())
            };

            return SendAsync<RewardCampaignGameResponse>(request);
        }
    }
}