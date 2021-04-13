using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class Base
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }
}