using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TGS.BettingSpeedTest.Messages.AsiaGaming;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest.Clients
{
    public interface IAsiaGamingApiClient
    {
        ValueTask<BetResponse> PlaceBetAsync(Guid tokenId, string playerCode, string externalRoundId);

        ValueTask<BetResponse> SettleBetAsync(Guid tokenId, string playerCode, string externalRoundId, decimal amount);
    }

    internal class AsiaGamingApiClient : BaseApiClient, IAsiaGamingApiClient
    {
        ValueTask<BetResponse> IAsiaGamingApiClient.PlaceBetAsync(Guid tokenId, string playerCode,
            string externalRoundId)
        {
            var url = AsiaGamingSettings.ApiEndpoint + AsiaGamingSettings.ApiPath.BetsPlaceAndSettle;
            var request = new PlaceBetRequest
            {
                Record = new()
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
            };

            var httpRequestMessage = GetHttpRequestMessage(HttpMethod.Post, url, request);
            return SendAndReadFromXmlAsync<BetResponse>(httpRequestMessage);
        }

        ValueTask<BetResponse> IAsiaGamingApiClient.SettleBetAsync(Guid tokenId, string playerCode,
            string externalRoundId,
            decimal amount)
        {
            var url = AsiaGamingSettings.ApiEndpoint + AsiaGamingSettings.ApiPath.BetsPlaceAndSettle;

            var request = new SettleBetRequest
            {
                Record = new()
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
                    TransactionIds = new[] {externalRoundId},
                    RoundId = externalRoundId,
                    TransactionType = (amount == 0 ? AsiaGamingSettings.Lose : AsiaGamingSettings.Win).ToUpper(),
                    Platform = AsiaGamingSettings.Platform,
                    RoundClosed = "true"
                }
            };

            var httpRequestMessage = GetHttpRequestMessage(HttpMethod.Post, url, request);
            return SendAndReadFromXmlAsync<BetResponse>(httpRequestMessage);
        }

        private static HttpRequestMessage GetHttpRequestMessage<TRequest>(HttpMethod method, string url,
            TRequest request) =>
            new(method, url)
            {
                Content = new StringContent(ObjectToXml(request), Encoding.UTF8, MediaTypeNames.Application.Xml)
            };

        private static string GetAsiaGamingPlayerCode(string playerCode) => AsiaGamingSettings.AgentCode + playerCode;

        private static string GetAsiaGamingTimestamp() =>
            DateTimeOffset.UtcNow.AddHours(-4).ToString(AsiaGamingSettings.TimestampFormat);

        private static string ObjectToXml<TRequest>(TRequest request)
        {
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");
            using var sw = new Utf8StringWriter();
            using var writer = XmlWriter.Create(sw);
            var serializer = new XmlSerializer(typeof(TRequest));
            serializer.Serialize(writer, request, emptyNamespaces);
            return sw.ToString();
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => new UTF8Encoding(false);
        }
    }
}