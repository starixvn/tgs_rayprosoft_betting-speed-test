using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class TransactionResponse : BaseResponse
    {
        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("retransmission")]
        public bool Retransmission { get; set; }
    }
}