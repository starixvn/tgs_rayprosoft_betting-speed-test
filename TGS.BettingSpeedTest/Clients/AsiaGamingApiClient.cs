using System;
using System.Threading.Tasks;
using TGS.BettingSpeedTest.Messages.AsiaGaming;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IAsiaGamingApiClient
    {
        Task<BetResponse> PlaceBetAsync(Guid tokenId, string playerCode, string externalRoundId);

        Task<BetResponse> SettleBetAsync(Guid tokenId, string playerCode, string externalRoundId, decimal amount);
    }

    internal class AsiaGamingApiClient : BaseApiClient, IAsiaGamingApiClient
    {
        Task<BetResponse> IAsiaGamingApiClient.PlaceBetAsync(Guid tokenId, string playerCode, string externalRoundId)
        {
            var url = AsiaGamingSettings.ApiEndpoint + AsiaGamingSettings.ApiPath.BetsPlaceAndSettle;

            return SendXmlAsync<PlaceBetRequest, BetResponse>(new PlaceBetRequest
            {
                Record = new PlaceBetRecord
                {
                    Amount = AsiaGamingSettings.BetAmount,
                    Timestamp = GetAsiaGamingTimestamp(),
                    GamePlatformType = AsiaGamingSettings.GamePlatformType,
                    GameRoomType = AsiaGamingSettings.GameRoomType,
                    GameType = AsiaGamingSettings.GameType,
                    BetType = "1",
                    TokenId = tokenId.ToString(),
                    Currency = AsiaGamingSettings.Currency,
                    PlayerCode = GetAsiaGamingPlayerCode(playerCode),
                    AgentCode = AsiaGamingSettings.AgentCode,
                    TransactionId = externalRoundId,
                    RoundId = externalRoundId,
                    TransactionType = AsiaGamingSettings.Bet.ToUpper(),
                    Platform = AsiaGamingSettings.Platform
                }
            }, url);
        }

        Task<BetResponse> IAsiaGamingApiClient.SettleBetAsync(Guid tokenId, string playerCode, string externalRoundId, decimal amount)
        {
            var url = AsiaGamingSettings.ApiEndpoint + AsiaGamingSettings.ApiPath.BetsPlaceAndSettle;

            return SendXmlAsync<SettleBetRequest, BetResponse>(new SettleBetRequest
            {
                Record = new SettleBetRecord
                {
                    WinAmount = amount,
                    BetAmount = -AsiaGamingSettings.BetAmount,
                    Timestamp = GetAsiaGamingTimestamp(),
                    GameType = AsiaGamingSettings.GameType,
                    BetId = Guid.NewGuid().ToString(),
                    TokenId = tokenId.ToString(),
                    Currency = AsiaGamingSettings.Currency,
                    PlayerCode = GetAsiaGamingPlayerCode(playerCode),
                    AgentCode = AsiaGamingSettings.AgentCode,
                    TransactionIds = new[] { externalRoundId },
                    RoundId = externalRoundId,
                    TransactionType = (amount == 0 ? AsiaGamingSettings.Lose : AsiaGamingSettings.Win).ToUpper(),
                    Platform = AsiaGamingSettings.Platform,
                    RoundClosed = "true"
                }
            }, url);
        }

        private string GetAsiaGamingPlayerCode(string playerCode)
        {
            return AsiaGamingSettings.AgentCode + playerCode;
        }

        private string GetAsiaGamingTimestamp()
        {
            return DateTimeOffset.UtcNow.AddHours(-4).ToString(AsiaGamingSettings.TimestampFormat);
        }
    }
}