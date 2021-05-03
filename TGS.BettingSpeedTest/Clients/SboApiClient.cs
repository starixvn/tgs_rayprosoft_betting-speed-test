using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Sbo;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface ISboApiClient
    {
        ValueTask<PlaceBetResponse> PlaceBetAsync(string playerCode, string externalRoundId);

        ValueTask<SettleBetResponse> SettleBetAsync(string playerCode, string externalRoundId, ResultType result,
            decimal amount);
    }

    internal sealed class SboApiClient : BaseApiClient, ISboApiClient
    {
        private readonly JsonSerializerOptions
            _jsonSerializerOptions = new() {WriteIndented = false};

        ValueTask<PlaceBetResponse> ISboApiClient.PlaceBetAsync(string playerCode, string externalRoundId)
        {
            var url = SboSettings.ApiEndpoint + SboSettings.ApiPath.PlaceBet;

            var request = new PlaceBetRequest
            {
                CompanyKey = SboSettings.CompanyKey,
                ProductType = SboSettings.ProductType,
                GameType = SboSettings.GameType, PlayerCode = playerCode,
                ExternalTransactionId = externalRoundId,
                ExternalRoundId = externalRoundId,
                Amount = 10,
                BetTime = DateTimeOffset.UtcNow
            };

            var value = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(value, Encoding.UTF8, "application/json")
            };

            return SendAndReadFromJsonAsync<PlaceBetResponse>(httpRequestMessage,
                jsonSerializerOptions: _jsonSerializerOptions);
        }

        ValueTask<SettleBetResponse> ISboApiClient.SettleBetAsync(string playerCode, string externalRoundId,
            ResultType result, decimal amount)
        {
            var url = SboSettings.ApiEndpoint + SboSettings.ApiPath.SettleBet;

            var request = new SettleBetRequest
            {
                CompanyKey = SboSettings.CompanyKey,
                ProductType = SboSettings.ProductType,
                GameType = SboSettings.GameType, PlayerCode = playerCode,
                ExternalTransactionId = externalRoundId,
                WinLoss = amount,
                Result = result,
                SettleTime = DateTimeOffset.UtcNow
            };

            var value = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(value, Encoding.UTF8, "application/json")
            };

            return SendAndReadFromJsonAsync<SettleBetResponse>(httpRequestMessage,
                jsonSerializerOptions: _jsonSerializerOptions);
        }
    }
}