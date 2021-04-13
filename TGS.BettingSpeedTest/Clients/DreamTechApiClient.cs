using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.DreamTech;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IDreamTechApiClient
    {
        Task<TransferResponse> PlaceAndSettleBetAsync(string playerToken, string playerCode, string externalTransactionId, string amount);
    }

    internal class DreamTechApiClient : BaseApiClient, IDreamTechApiClient
    {
        Task<TransferResponse> IDreamTechApiClient.PlaceAndSettleBetAsync(string playerToken, string playerCode, string externalTransactionId, string amount)
        {
            var url = DreamTechSettings.ApiEndpoint;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransferRequest>(new TransferRequest
                {
                    ExternalGameId = "dragonball",
                    PlayerCode = playerCode,
                    Currency = "CNY",
                    BetWins = amount,
                    Method = "bet",
                    Business = DreamTechSettings.Business,
                    BetPrice = "0.25",
                    BetId = externalTransactionId,
                    RoundClosed = "0",
                    CreateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PlayerToken = playerToken,
                    BypassSignatureVerify = true
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<TransferResponse>(request);
        }

    }
}