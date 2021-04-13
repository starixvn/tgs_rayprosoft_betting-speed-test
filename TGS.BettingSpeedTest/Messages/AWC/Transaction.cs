using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGS.BettingSpeedTest.Messages.AWC
{
    public sealed class Transaction
    {
        [JsonProperty(PropertyName = "txId")] 
        public string ExternalBetId { get; set; }

        [JsonProperty(PropertyName = "platformTxId")]
        public string ExternalTransactionId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string PlayerCode { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public string PlatformName { get; set; }

        [JsonProperty(PropertyName = "gameType")]
        public string GameType { get; set; }

        [JsonProperty(PropertyName = "gameCode")]
        public string ExternalGameId { get; set; }

        [JsonProperty(PropertyName = "gameName")]
        public string GameName { get; set; }

        [JsonProperty(PropertyName = "betType")]
        public string BetType { get; set; }

        [JsonProperty(PropertyName = "betAmount")]
        public decimal BetAmount { get; set; }

        [JsonProperty(PropertyName = "betTime")]
        public DateTimeOffset BetTime { get; set; }

        [JsonProperty(PropertyName = "roundId")]
        public string ExternalRoundId { get; set; }

        [JsonProperty(PropertyName = "winAmount")]
        public decimal WinAmount { get; set; }

        [JsonProperty(PropertyName = "turnover")]
        public decimal Turnover { get; set; }

        [JsonProperty(PropertyName = "updateTime")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty(PropertyName = "gameInfo")]
        public JObject GameInfo { get; set; }
    }
}
