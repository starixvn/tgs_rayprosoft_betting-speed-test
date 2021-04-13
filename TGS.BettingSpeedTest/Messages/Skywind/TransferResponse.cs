using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Skywind
{
    public class TransferResponse : ApiResponse
    {
        [JsonProperty("error_msg")]
        public string ErrorMessage { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("trx_id")]
        public string ExternalTransactionId { get; set; }
    }
}
