using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Tyche;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface ITycheApiClient
    {
        Task<TransferResponse> PlaceBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount);
        Task<TransferResponse> SettleBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount);
    }

    internal class TycheApiClient : BaseApiClient, ITycheApiClient
    {
        Task<TransferResponse> ITycheApiClient.PlaceBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount)
        {
            var url = TycheSettings.ApiEndpoint;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransferRequest>(new TransferRequest
                {
                    Method = "debit",
                    Data = new TransferRequest.DataRequest
                    {
                        ExternalGameId = "win-win-won",
                        PlayerId = playerId,
                        Currency = "CNY",
                        Amount = amount,
                        ExternalTransactionId = externalTransactionId,
                        ExternalRoundId = roundId,
                        RoundClosed = false,
                        PlayerToken = playerToken,
                        TimeStamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffZ"),
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TycheSettings.Authorization);
            return SendAsync<TransferResponse>(request);
        }

        Task<TransferResponse> ITycheApiClient.SettleBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount)
        {
            var url = TycheSettings.ApiEndpoint;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransferRequest>(new TransferRequest
                {
                    Method = "credit",
                    Data = new TransferRequest.DataRequest
                    {
                        ExternalGameId = "win-win-won",
                        PlayerId = playerId,
                        Currency = "CNY",
                        Amount = amount,
                        ExternalTransactionId = externalTransactionId,
                        ExternalRoundId = roundId,
                        RoundClosed = true,
                        PlayerToken = playerToken,
                        TimeStamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffZ")
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TycheSettings.Authorization);
            return SendAsync<TransferResponse>(request);
        }

    }
}