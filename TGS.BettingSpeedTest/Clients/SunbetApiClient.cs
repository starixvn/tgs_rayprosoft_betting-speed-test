using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Extensions;
using TGS.BettingSpeedTest.Messages;
using TGS.BettingSpeedTest.Messages.GameProvider;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface ISunbetApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync();

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId);

        Task<PlaceBetResponse> PlaceBetsAsync(string accessToken, string playerToken,
            PlaceBetRequest.TransactionRequest[] transactionRequests);

        Task<BatchResponse> SettleBetAsync(string accessToken, Guid playerId, string externalTransactionId, string externalReferencedTransactionId, string description = "");

        Task<BatchResponse> SettleBetsAsync(string accessToken, BatchRequest.BatchRequestTransaction[] batchRequestTransactions);

        Task<BatchResponse> LoseBetAsync(string accessToken, Guid playerId, string externalTransactionId, string externalReferencedTransactionId, string description = "");
    }

    internal class SunbetApiClient : BaseApiClient, ISunbetApiClient
    {
        Task<GetAccessTokenResponse> ISunbetApiClient.GetAccessTokenAsync()
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", SunbetSettings.OAuth.ClientId),
                    new KeyValuePair<string, string>("client_secret", SunbetSettings.OAuth.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", SunbetSettings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", SunbetSettings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<PlaceBetResponse> ISunbetApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId)
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.BetsPlace;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceBetRequest>(new PlaceBetRequest
                {
                    authtoken = playerToken,
                    transactions = new List<PlaceBetRequest.TransactionRequest>
                    {
                        new PlaceBetRequest.TransactionRequest
                        {
                            amt = 1,
                            cur = OperatorSettings.Player.Currency,
                            txid = externalTransactionId,
                            roundid = externalTransactionId,
                            betrecid = externalTransactionId,
                            gameid = SunbetSettings.TestGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<PlaceBetResponse> ISunbetApiClient.PlaceBetsAsync(string accessToken, string playerToken, PlaceBetRequest.TransactionRequest[] transactionRequests)
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.BetsPlace;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceBetRequest>(new PlaceBetRequest
                {
                    authtoken = playerToken,
                    transactions = transactionRequests.ToList()
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<BatchResponse> ISunbetApiClient.SettleBetAsync(
            string accessToken, Guid playerId, string externalTransactionId, string externalReferencedTransactionId, string description)
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<BatchRequest>(new BatchRequest
                {
                    timestamp = DateTimeOffset.Now,
                    securitykey = SunbetSettings.OAuth.SecurityKey,
                    batchid = Guid.NewGuid().ToString(),
                    transactions = new List<BatchRequest.BatchRequestTransaction>
                    {
                        new BatchRequest.BatchRequestTransaction
                        {
                            amt = 10,
                            cur = OperatorSettings.Player.Currency,
                            txid = externalTransactionId,
                            txtype = TransactionTypes.Win,
                            txrefid = externalReferencedTransactionId,
                            roundid = externalReferencedTransactionId,
                            betrecid = externalTransactionId,
                            userid = playerId,
                            gameid = SunbetSettings.TestGameId,
                            desc = description
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<BatchResponse>(request);
        }

        Task<BatchResponse> ISunbetApiClient.SettleBetsAsync(string accessToken, BatchRequest.BatchRequestTransaction[] batchRequestTransactions)
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<BatchRequest>(new BatchRequest
                {
                    timestamp = DateTimeOffset.Now,
                    securitykey = SunbetSettings.OAuth.SecurityKey,
                    batchid = Guid.NewGuid().ToString(),
                    transactions = batchRequestTransactions.ToList()
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<BatchResponse>(request);
        }

        Task<BatchResponse> ISunbetApiClient.LoseBetAsync(
            string accessToken, Guid playerId, string externalTransactionId, string externalReferencedTransactionId, string description)
        {
            var url = SunbetSettings.ApiEndpoint + SunbetSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<BatchRequest>(new BatchRequest
                {
                    timestamp = DateTimeOffset.Now,
                    securitykey = SunbetSettings.OAuth.SecurityKey,
                    batchid = Guid.NewGuid().ToString(),
                    transactions = new List<BatchRequest.BatchRequestTransaction>
                    {
                        new BatchRequest.BatchRequestTransaction
                        {
                            amt = 10,
                            cur = OperatorSettings.Player.Currency,
                            txid = externalTransactionId,
                            txtype = TransactionTypes.Loss,
                            txrefid = externalReferencedTransactionId,
                            roundid = externalReferencedTransactionId,
                            betrecid = externalTransactionId,
                            userid = playerId,
                            gameid = SunbetSettings.TestGameId,
                            desc = description
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<BatchResponse>(request);
        }
    }
}