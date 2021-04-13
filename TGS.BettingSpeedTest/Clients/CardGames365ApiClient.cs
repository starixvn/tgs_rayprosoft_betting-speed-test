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
    public interface ICardGames365ApiClient
    {
        Task<GetAccessTokenResponse> GetAccessTokenAsync();

        Task<LockTransactionResponse> LockTransactionAsync(string accessToken, string playerToken, string externalTransactionId, string externalRoundId);

        Task<PlaceBetResponse> PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId);
        
        Task<SettleBetResponse> SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, string externalReferencedTransactionId, int amount, bool returnBet = false);
    }

    internal class CardGames365ApiClient : BaseApiClient, ICardGames365ApiClient
    {
        private const int BetAmount = 1,
                          LockAmount = BetAmount * 5;

        Task<GetAccessTokenResponse> ICardGames365ApiClient.GetAccessTokenAsync()
        {
            var url = CardGames365Settings.ApiEndpoint + CardGames365Settings.ApiPath.OAuthToken;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", CardGames365Settings.OAuth.ClientId),
                    new KeyValuePair<string, string>("client_secret", CardGames365Settings.OAuth.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", CardGames365Settings.OAuth.GrantType),
                    new KeyValuePair<string, string>("scope", CardGames365Settings.OAuth.Scope) })
            };

            return SendAsync<GetAccessTokenResponse>(request);
        }

        Task<LockTransactionResponse> ICardGames365ApiClient.LockTransactionAsync(string accessToken, string playerToken, string externalTransactionId, string externalRoundId)
        {
            var url = CardGames365Settings.ApiEndpoint + CardGames365Settings.ApiPath.LockTransaction;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<LockTransactionRequest>(new LockTransactionRequest
                {
                    authtoken = playerToken,
                    amt = LockAmount,
                    cur = CardGames365Settings.Currency,
                    txid = externalTransactionId,
                    roundid = externalRoundId,
                    gameid = CardGames365Settings.TestGameId,
                    timestamp = DateTimeOffset.Now.ToIso8601String(),
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<LockTransactionResponse>(request);
        }

        Task<PlaceBetResponse> ICardGames365ApiClient.PlaceBetAsync(string accessToken, string playerToken, string externalTransactionId)
        {
            var url = CardGames365Settings.ApiEndpoint + CardGames365Settings.ApiPath.BetsPlace;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceBetRequest>(new PlaceBetRequest
                {
                    authtoken = playerToken,
                    transactions = new List<PlaceBetRequest.TransactionRequest>
                    {
                        new PlaceBetRequest.TransactionRequest
                        {
                            amt = BetAmount,
                            cur = CardGames365Settings.Currency,
                            uselockedamt = true,
                            txid = externalTransactionId,
                            roundid = externalTransactionId,
                            betrecid = externalTransactionId,
                            gameid = CardGames365Settings.TestGameId,
                            timestamp = DateTimeOffset.Now.ToIso8601String(),
                            desc = string.Empty
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<PlaceBetResponse>(request);
        }
        
        Task<SettleBetResponse> ICardGames365ApiClient.SettleBetAsync(string accessToken, string playerToken, string externalTransactionId, 
                                                                      string externalReferencedTransactionId, int amount, bool returnBet)
        {
            var url = CardGames365Settings.ApiEndpoint + CardGames365Settings.ApiPath.BetsSettle;
            var transactions = new List<SettleBetRequest.TransactionRequest>
            {
                new SettleBetRequest.TransactionRequest
                {
                    amt = amount,
                    cur = CardGames365Settings.Currency,
                    txid = externalTransactionId,
                    txtype = amount == 0 ? TransactionTypes.Loss : TransactionTypes.Win,
                    txrefid = externalReferencedTransactionId,
                    roundid = externalReferencedTransactionId,
                    betrecid = externalTransactionId,
                    timestamp = DateTimeOffset.Now.ToIso8601String(),
                    roundclosed = !returnBet,
                    desc = string.Empty
                }
            };

            if (returnBet)
            {
                transactions.Add(new SettleBetRequest.TransactionRequest
                {
                    amt = BetAmount,
                    cur = CardGames365Settings.Currency,
                    txid = externalTransactionId,
                    txtype = TransactionTypes.Return,
                    txrefid = externalReferencedTransactionId,
                    roundid = externalReferencedTransactionId,
                    betrecid = externalTransactionId,
                    timestamp = DateTimeOffset.Now.ToIso8601String(),
                    roundclosed = true,
                    desc = string.Empty
                });
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<SettleBetRequest>(new SettleBetRequest
                {
                    authtoken = playerToken,
                    unlockedamt = LockAmount - BetAmount,
                    transactions = transactions
                }, new JsonMediaTypeFormatter())
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return SendAsync<SettleBetResponse>(request);
        }
    }
}