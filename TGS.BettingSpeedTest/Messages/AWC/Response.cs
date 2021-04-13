using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.AWC
{
    public sealed class Response
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "balance", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Balance { get; set; }

        [JsonProperty(PropertyName = "balanceTs", NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }
    }
}
