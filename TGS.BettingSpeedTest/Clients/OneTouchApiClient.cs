using System;
using System.Collections.Generic;
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
    public interface IOneTouchApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync();

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId);

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalGameId, string externalRoundId, string externalTransactionId);

        Task<SettleBetResponse> SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId);

        Task<LoseBetResponse> LoseBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId);
    }

    internal sealed class OneTouchApiClient : BaseApiClient, IOneTouchApiClient
    {
        Task<GetAccessTokenResponse> IOneTouchApiClient.GetAccessTokenAsync()
        {
            var url = OneTouchSettings.ApiEndpoint + OneTouchSettings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", OneTouchSettings.OAuth.ClientId),
                    new KeyValuePair<string, string>("client_secret", OneTouchSettings.OAuth.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", OneTouchSettings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", OneTouchSettings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<PlaceBetResponse> IOneTouchApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId)
        {
            var url = OneTouchSettings.ApiEndpoint + OneTouchSettings.ApiPath.BetsPlace;

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
                            gameid = OneTouchSettings.TestGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<PlaceBetResponse> IOneTouchApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalGameId, string externalRoundId, string externalTransactionId)
        {
            var url = OneTouchSettings.ApiEndpoint + OneTouchSettings.ApiPath.BetsPlace;

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
                            roundid = externalRoundId,
                            betrecid = externalTransactionId,
                            gameid = externalGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<SettleBetResponse> IOneTouchApiClient.SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId)
        {
            var url = OneTouchSettings.ApiEndpoint + OneTouchSettings.ApiPath.BetsSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<SettleBetRequest>(new SettleBetRequest
                {
                    authtoken = playerToken,
                    transactions = new List<SettleBetRequest.TransactionRequest>
                    {
                        new SettleBetRequest.TransactionRequest
                        {
                            amt = 10,
                            cur = OperatorSettings.Player.Currency,
                            txid = externalTransactionId,
                            txtype = TransactionTypes.Win,
                            txrefid = externalReferencedTransactionId,
                            roundid = externalReferencedTransactionId,
                            betrecid = externalTransactionId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<SettleBetResponse>(request);
        }

        Task<LoseBetResponse> IOneTouchApiClient.LoseBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId)
        {
            var url = OneTouchSettings.ApiEndpoint + OneTouchSettings.ApiPath.BetsLose;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<LoseBetRequest>(new LoseBetRequest
                {
                    authtoken = playerToken,
                    transactions = new List<LoseBetRequest.TransactionRequest>
                    {
                        new LoseBetRequest.TransactionRequest
                        {
                            cur = OperatorSettings.Player.Currency,
                            txid = externalTransactionId,
                            txrefid = externalReferencedTransactionId,
                            roundid = externalReferencedTransactionId,
                            betrecid = externalTransactionId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<LoseBetResponse>(request);
        }
    }
}