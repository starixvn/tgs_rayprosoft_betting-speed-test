using System;
using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class CommonDataRequest
    {
        [JsonProperty("username")]
        public Guid PlayerId { get; set; }

        [JsonProperty("sessionToken")]
        public string PlayerToken { get; set; }

        [JsonProperty("gameId")]
        public string ExternalGameId { get; set; }

        [JsonProperty("roundCompleted")]
        public bool RoundClosed { get; set; }
    }
}
