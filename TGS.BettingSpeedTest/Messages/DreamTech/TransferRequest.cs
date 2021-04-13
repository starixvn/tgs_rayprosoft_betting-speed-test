using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGS.BettingSpeedTest.Messages.DreamTech
{
    public class TransferRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("business")]
        public string Business { get; set; }

        [JsonProperty("betId")]
        public string BetId { get; set; }

        [JsonProperty("playerName")]
        public string PlayerCode { get; set; }

        [JsonProperty("gameCode")]
        public string ExternalGameId { get; set; }

        [JsonProperty("parentId")]
        public JObject ParentId { get; set; }

        [JsonProperty("isComplete")]
        public string RoundClosed { get; set; }

        [JsonProperty("betLine")]
        public string BetLine { get; set; }

        [JsonProperty("betWins")]
        public string BetWins { get; set; }

        public string BetPrice { get; set; }

        public string CreateTime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("bypasssignatureverify")]
        public bool BypassSignatureVerify { get; set; }

        [JsonProperty("token")]
        public string PlayerToken { get; set; }
    }
}