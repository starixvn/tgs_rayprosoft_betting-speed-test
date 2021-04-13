using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Skywind;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface ISkywindApiClient
    {
        Task<TransferResponse> PlaceBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount);
        Task<TransferResponse> SettleBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount);
    }

    internal class SkywindApiClient : BaseApiClient, ISkywindApiClient
    {
        Task<TransferResponse> ISkywindApiClient.PlaceBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount)
        {
            var url = SkywindSettings.ApiEndpoint + SkywindSettings.ApiPath.BetsPlace;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(ConvertToKeyValue(new TransferRequest
                {
                    Merch_id = SkywindSettings.MerchantCode,
                    Merch_pwd = SkywindSettings.MerchantPassword,
                    Cust_id = playerId,
                    Cust_session_id = playerToken,
                    Game_code = "sw_mer",
                    Amount = amount,
                    Currency_code = "CNY",
                    Trx_id = externalTransactionId,
                    Game_id = roundId,
                    Event_id = "0",
                    Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                    Platform = "web",
                    Game_type = "normal",
                    Event_type = "bet"
                }))
            };

            return SendAsync<TransferResponse>(request);
        }

        Task<TransferResponse> ISkywindApiClient.SettleBetAsync(string playerToken, Guid playerId, string roundId, string externalTransactionId, decimal amount)
        {
            var url = SkywindSettings.ApiEndpoint + SkywindSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {

                Content = new FormUrlEncodedContent(ConvertToKeyValue(new TransferRequest
                {
                    Merch_id = SkywindSettings.MerchantCode,
                    Merch_pwd = SkywindSettings.MerchantPassword,
                    Cust_id = playerId,
                    Cust_session_id = playerToken,
                    Game_code = "sw_mer",
                    Amount = amount,
                    Currency_code = "CNY",
                    Trx_id = externalTransactionId,
                    Game_id = roundId,
                    Event_id = "0",
                    Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                    Platform = "web",
                    Game_type = "normal",
                    Event_type = "win"
                }))
            };

            return SendAsync<TransferResponse>(request);
        }
    }
}