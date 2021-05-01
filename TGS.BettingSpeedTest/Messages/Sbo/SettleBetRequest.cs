using System;
using System.Text.Json.Serialization;

namespace TGS.BettingSpeedTest.Messages.Sbo
{
    public record SettleBetRequest
    {
        [JsonPropertyName("companyKey")]
        public string CompanyKey { get; init; }

        [JsonPropertyName("userName")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("productType")]
        public int ProductType { get; init; }

        [JsonPropertyName("gameType")]
        public int GameType { get; init; }

        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("winloss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("resultType")]
        public ResultType Result { get; init; }

        [JsonPropertyName("resultTime")]
        public DateTimeOffset SettleTime { get; init; }
    }

    public enum ResultType
    {
        Win = 0,
        Loss = 1,
        Tie = 2
    }
}