using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.XPG
{
    public class SuccessResponse
    {
        [JsonProperty("d")]
        public BalanceResponse Data { get; set; }
    }

    public class BalanceResponse : Error
    {
        [JsonProperty("Data", NullValueHandling = NullValueHandling.Ignore)]
        public decimal[] Balances { get; set; }
    }
}