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
    public interface IDopamineApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync();

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId);

        Task<SettleBetResponse> SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId, int transactionType, bool enableJackpot = false);

    }

    internal class DopamineApiClient : BaseApiClient, IDopamineApiClient
    {
        Task<GetAccessTokenResponse> IDopamineApiClient.GetAccessTokenAsync()
        {
            var url = DopamineSettings.ApiEndpoint + DopamineSettings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", DopamineSettings.OAuth.ClientId),
                    new KeyValuePair<string, string>("client_secret", DopamineSettings.OAuth.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", DopamineSettings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", DopamineSettings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<PlaceBetResponse> IDopamineApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId)
        {
            var url = DopamineSettings.ApiEndpoint + DopamineSettings.ApiPath.BetsPlace;

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
                            gameid = DopamineSettings.TestSlotGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }

        Task<SettleBetResponse> IDopamineApiClient.SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId, int transactionType, bool enableJackpot)
        {
            var url = DopamineSettings.ApiEndpoint + DopamineSettings.ApiPath.BetsSettle;

            var jackpotContribution = new List<JackpotContribution>
            {
                new JackpotContribution
                {
                    jpid = "4001",
                    jpcur = "RMB",
                    jprate = 1.0000000000m,
                    jpamt = 0.010000m,
                    cvtamt = 0.010000m,
                    jpbal = 610902.79m
                },
                new JackpotContribution
                {
                    jpid = "4002",
                    jpcur = "RMB",
                    jprate = 1.0000000000m,
                    jpamt = 0.022500m,
                    cvtamt = 0.022500m,
                    jpbal = 70685.05m
                },
                new JackpotContribution
                {
                    jpid = "4003",
                    jpcur = "RMB",
                    jprate = 1.0000000000m,
                    jpamt = 0.012500m,
                    cvtamt = 0.012500m,
                    jpbal = 1473.70m
                },
                new JackpotContribution
                {
                    jpid = "4004",
                    jpcur = "RMB",
                    jprate = 1.0000000000m,
                    jpamt = 0.005000m,
                    cvtamt = 0.005000m,
                    jpbal = 1380.00m
                }
            };

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
                            txtype = transactionType,
                            txrefid = externalReferencedTransactionId,
                            roundid = externalReferencedTransactionId,
                            betrecid = externalTransactionId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty,
                            jpcontribs = enableJackpot ? jackpotContribution.ToArray() :default
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<SettleBetResponse>(request);
        }

    }
}