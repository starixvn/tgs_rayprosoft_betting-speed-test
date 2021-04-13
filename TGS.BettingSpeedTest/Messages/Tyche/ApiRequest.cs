using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class ApiRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        public GameProviderData GameProviderData { get; set; }
    }
}
