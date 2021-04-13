using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.MikiWorld
{
    public sealed class TransactionResponse
    {
        [JsonProperty("SeqNo")]
        public string SeqNo { get; set; }

        [JsonProperty("Cmd")]
        public string Event { get; set; }

        [JsonProperty]
        public long Timestamp { get; set; }

        [JsonProperty("PlayerName")]
        public string UserId { get; set; }

        [JsonProperty]
        public int Status { get; set; }
        
        [JsonProperty("Money")]
        public decimal Balance { get; set; }

        [JsonProperty("MoneyBefore")]
        public decimal BalanceBefore { get; set; }
    }
}
