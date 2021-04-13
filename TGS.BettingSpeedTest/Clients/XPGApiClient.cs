using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.XPG;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IXPGApiClient
    {
        Task<SuccessResponse> PlaceBetAsync(string playerToken, string playerCode, string externalRoundId);

        Task<SuccessResponse> PlaceBetAsync(string playerToken, string playerCode, string externalRoundId, int sequence);

        Task<SuccessResponse> SettleBetAsync(string playerToken, string playerCode, string externalRoundId, decimal amount, bool multiBets);
    }

    internal class XPGApiClient : BaseApiClient, IXPGApiClient
    {
        Task<SuccessResponse> IXPGApiClient.PlaceBetAsync(string playerToken, string playerCode, string externalRoundId)
        {
            var url = XPGSettings.ApiEndpoint + XPGSettings.ApiPath.BetsPlace;
            var sequence = "1";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceBetRequest>(new PlaceBetRequest
                {
                    PlayerToken = playerToken,
                    PlayerCode = playerCode,
                    GameId = XPGSettings.TestGameId,
                    RoundId = externalRoundId,
                    Sequence = sequence,
                    Amount = 1,
                    DebitDetails = string.Format(XPGSettings.DebitDetail, sequence),
                    OperatorId = XPGSettings.OperatorId
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<SuccessResponse>(request);
        }

        Task<SuccessResponse> IXPGApiClient.PlaceBetAsync(string playerToken, string playerCode, string externalRoundId, int sequence)
        {
            var url = XPGSettings.ApiEndpoint + XPGSettings.ApiPath.BetsPlace;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceBetRequest>(new PlaceBetRequest
                {
                    PlayerToken = playerToken,
                    PlayerCode = playerCode,
                    GameId = XPGSettings.TestGameId,
                    RoundId = externalRoundId,
                    Sequence = sequence.ToString(),
                    Amount = 1,
                    DebitDetails = string.Format(XPGSettings.DebitDetail, sequence),
                    OperatorId = XPGSettings.OperatorId
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<SuccessResponse>(request);
        }

        Task<SuccessResponse> IXPGApiClient.SettleBetAsync(string playerToken, string playerCode, string externalRoundId, decimal amount, bool multiBets)
        {
            var url = XPGSettings.ApiEndpoint + XPGSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransactionRequest>(new TransactionRequest
                {
                    PlayerToken = playerToken,
                    PlayerCode = playerCode,
                    GameId = XPGSettings.TestGameId,
                    RoundId = externalRoundId,
                    Amount = amount,
                    CreditDetails = string.Format(multiBets ? XPGSettings.CreditDetailWithMultiBets : XPGSettings.CreditDetail, amount),
                    OperatorId = XPGSettings.OperatorId
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<SuccessResponse>(request);
        }
    }
}