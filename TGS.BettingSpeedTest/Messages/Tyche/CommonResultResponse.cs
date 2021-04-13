using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class CommonResultResponse
    {
        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("cashBalance")]
        public decimal CashBalance { get; set; }

        [JsonProperty("bonusBalance")]
        public decimal BonusBalance;

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
