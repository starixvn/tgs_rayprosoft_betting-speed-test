using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.MikiWorld;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IMikiWorldApiClient
    {
        Task<TransactionResponse> PlaceBetAsync(string playerToken, string playerCode, string externalTransactionId);
        
        Task<TransactionResponse> SettleBetAsync(string playerToken, string playerCode, string externalTransactionId, decimal amount);
    }

    internal class MikiWorldApiClient : BaseApiClient, IMikiWorldApiClient
    {
        Task<TransactionResponse> IMikiWorldApiClient.PlaceBetAsync(string playerToken, string playerCode, string externalTransactionId)
        {
            var url = MikiWorldSettings.ApiEndpoint + MikiWorldSettings.ApiPath.BetsPlaceAndSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransactionRequest>(new TransactionRequest
                {
                    Event = MikiWorldSettings.Event,
                    SeqNo = Guid.NewGuid().ToString(),
                    Currency = MikiWorldSettings.Currency,
                    ChannelId = MikiWorldSettings.ChannelId,
                    RoundId = externalTransactionId,
                    TransactionId = externalTransactionId,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    PlayerToken = playerToken,
                    GameId = MikiWorldSettings.GameId,
                    Amount = -MikiWorldSettings.BetAmount,
                    BetItem = new Dictionary<string, decimal>
                    {
                        {"31", -MikiWorldSettings.BetAmount}
                    },
                    BetTimeString = DateTimeOffset.Now.ToString(MikiWorldSettings.TimestampFormat),
                    Type = MikiWorldSettings.Bet,
                    BypassRSAVerify = true
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<TransactionResponse>(request);
        }

        Task<TransactionResponse> IMikiWorldApiClient.SettleBetAsync(string playerToken, string playerCode, string externalTransactionId, decimal amount)
        {
            var url = MikiWorldSettings.ApiEndpoint + MikiWorldSettings.ApiPath.BetsPlaceAndSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransactionRequest>(new TransactionRequest
                {
                    Event = MikiWorldSettings.Event,
                    SeqNo = Guid.NewGuid().ToString(),
                    Currency = MikiWorldSettings.Currency,
                    ChannelId = MikiWorldSettings.ChannelId,
                    RoundId = externalTransactionId,
                    TransactionId = externalTransactionId,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    PlayerToken = playerToken,
                    Payout = amount,
                    PayoutDetail = new Dictionary<string, decimal>
                    {
                        {"31", amount}
                    },
                    SourceResult = new []{ "14"},
                    PayoutTimeString = DateTimeOffset.Now.ToString(MikiWorldSettings.TimestampFormat),
                    Type = MikiWorldSettings.Settle,
                    BypassRSAVerify = true
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<TransactionResponse>(request);
        }
    }
}