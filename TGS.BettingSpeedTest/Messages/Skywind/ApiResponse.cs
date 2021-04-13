using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Skywind
{
    public class ApiResponse
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }
    }
}
