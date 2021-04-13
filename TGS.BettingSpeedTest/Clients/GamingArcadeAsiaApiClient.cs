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
    public interface IGamingArcadeAsiaApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync();

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId);

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalGameId, string externalRoundId, string externalTransactionId);

        Task<SettleBetResponse> SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId);

        Task<SettleBetResponse> LoseBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId);
    }

    internal class GamingArcadeAsiaApiClient : BaseApiClient, IGamingArcadeAsiaApiClient
    {
        Task<GetAccessTokenResponse> IGamingArcadeAsiaApiClient.GetAccessTokenAsync()
        {
            var url = GamingArcadeAsiaSettings.ApiEndpoint + GamingArcadeAsiaSettings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", GamingArcadeAsiaSettings.OAuth.ClientId),
                    new KeyValuePair<string, string>("client_secret", GamingArcadeAsiaSettings.OAuth.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", GamingArcadeAsiaSettings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", GamingArcadeAsiaSettings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<PlaceBetResponse> IGamingArcadeAsiaApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId)
        {
            var url = GamingArcadeAsiaSettings.ApiEndpoint + GamingArcadeAsiaSettings.ApiPath.BetsPlace;

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
                            gameid = GamingArcadeAsiaSettings.TestSlotGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<PlaceBetResponse> IGamingArcadeAsiaApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalGameId, string externalRoundId, string externalTransactionId)
        {
            var url = GamingArcadeAsiaSettings.ApiEndpoint + GamingArcadeAsiaSettings.ApiPath.BetsPlace;

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

        Task<SettleBetResponse> IGamingArcadeAsiaApiClient.SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId)
        {
            var url = GamingArcadeAsiaSettings.ApiEndpoint + GamingArcadeAsiaSettings.ApiPath.BetsSettle;
            
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

        Task<SettleBetResponse> IGamingArcadeAsiaApiClient.LoseBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId)
        {
            var url = GamingArcadeAsiaSettings.ApiEndpoint + GamingArcadeAsiaSettings.ApiPath.BetsSettle;
            
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
                            txtype = TransactionTypes.Loss,
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
    }
}