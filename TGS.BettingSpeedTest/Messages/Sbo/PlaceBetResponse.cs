using System.Text.Json.Serialization;

namespace TGS.BettingSpeedTest.Messages.Sbo
{
    public record PlaceBetResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("accountName")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("balance")]
        public decimal? Balance { get; init; }

        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; init; }
    }
}