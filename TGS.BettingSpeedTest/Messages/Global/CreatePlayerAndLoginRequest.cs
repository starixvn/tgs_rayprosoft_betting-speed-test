using System.Text.Json.Serialization;

namespace TGS.BettingSpeedTest.Messages.Global
{
    public record CreatePlayerAndLoginRequest
    {
        [JsonPropertyName("brandcode")]
        public string BrandCode { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("initwalletamount")]
        public decimal InitialWalletAmount { get; init; }
    }
}