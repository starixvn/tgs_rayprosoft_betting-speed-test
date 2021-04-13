using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class GameDataRequest : CommonDataRequest
    {
        [JsonProperty("gameRoundId")]
        public string ExternalRoundId { get; set; }

        [JsonProperty("transactionId")]
        public string ExternalTransactionId { get; set; }
    }
}
