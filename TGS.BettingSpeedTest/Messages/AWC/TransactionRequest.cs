using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.AWC
{
    public class TransactionRequest
    {
        [JsonProperty("key")] 
        public string Key { get; set; }

        [JsonProperty("message")] 
        public string Message { get; set; }

        [JsonProperty("extension1")] 
        public string Extension { get; set; }
    }
}
