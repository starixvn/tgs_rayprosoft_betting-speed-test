using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class BaseResponse : Base
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}