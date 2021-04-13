using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.Evolution;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IEvolutionApiClient
    {
        Task<TransactionResponse> DebitAsync(string playerToken, Guid playerId, string externalTransactionId);
        
        Task<PlaceAndSettleBetResponse> PlaceAndSettleBetAsync(string playerToken, Guid playerId, string externalTransactionId, decimal winAmount);
    }

    internal class EvolutionApiClient : BaseApiClient, IEvolutionApiClient
    {
        Task<TransactionResponse> IEvolutionApiClient.DebitAsync(string playerToken, Guid playerId,
            string externalTransactionId)
        {
            var url = EvolutionSettings.ApiEndpoint + EvolutionSettings.ApiPath.Debit;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<TransactionRequest>(new TransactionRequest
                {
                    UserId = playerId.ToString(),
                    Token = playerToken,
                    Currency = EvolutionSettings.Currency,
                    Uuid = Guid.NewGuid().ToString(),
                    Transaction = new Transaction
                    {
                        Id = externalTransactionId,
                        EvolutionTransactionId = Guid.NewGuid().ToString(),
                        Amount = EvolutionSettings.BetAmount
                    },
                    Game = new Game
                    {
                        RoundId = externalTransactionId,
                        Type = EvolutionSettings.TestGameType,
                        Details = new GameDetails
                        {
                            Table = new GameTable
                            {
                                Id = EvolutionSettings.TestTableId
                            }
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<TransactionResponse>(request);
        }
        
        Task<PlaceAndSettleBetResponse> IEvolutionApiClient.PlaceAndSettleBetAsync(string playerToken, Guid playerId, string externalTransactionId, decimal winAmount)
        {
            var url = EvolutionSettings.ApiEndpoint + EvolutionSettings.ApiPath.BetsPlaceAndSettle;

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<PlaceAndSettleBetRequest>(new PlaceAndSettleBetRequest
                {
                    UserId = playerId.ToString(),
                    Token = playerToken,
                    Currency = EvolutionSettings.Currency,
                    Transaction = new HistoryGame
                    {
                        RoundId = externalTransactionId,
                        ClosedOn = DateTime.Now,
                        GameType = EvolutionSettings.TestGameType,
                        Table = new GameTable
                        {
                            Id = EvolutionSettings.TestTableId
                        },
                        Participants = new[]
                        {
                            new Participant
                            {
                                Currency = EvolutionSettings.Currency,
                                PlayerId = playerId,
                                PlayerToken = playerToken,
                                BetDetails = new []
                                {
                                    new BetDetail
                                    {
                                        TransactionId = externalTransactionId,
                                        BetAmount = EvolutionSettings.BetAmount,
                                        BetOn = DateTimeOffset.Now,
                                        BetType = "ROU_12Red",
                                        WinAmount = winAmount
                                    }
                                }
                            }
                        }
                    }
                }, new JsonMediaTypeFormatter())
            };

            return SendAsync<PlaceAndSettleBetResponse>(request);
        }

    }
}