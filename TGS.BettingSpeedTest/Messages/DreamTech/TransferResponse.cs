using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.DreamTech
{
    public class TransferResponse
    {
        [JsonProperty("CODE")]
        public string Code { get; set; }
    }
}