using System;
using System.Text.Json.Serialization;

namespace TGS.BettingSpeedTest.Messages.Sbo
{
    public record PlaceBetRequest
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

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("betTime")]
        public DateTimeOffset BetTime { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
    }
}