using System.Collections.Generic;
using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.MikiWorld
{
    public sealed class TransactionRequest
    {
        [JsonProperty("ChannelIdn")]
        public int ChannelId { get; set; }
        
        [JsonProperty]
        public string SeqNo { get; set; }

        [JsonProperty("PlayerName")]
        public string UserId { get; set; }
        
        [JsonProperty]
        public long Timestamp { get; set; }
        
        [JsonProperty("Cmd")]
        public string Event { get; set; }
        
        [JsonProperty("SessionToken")]
        public string PlayerToken { get; set; }
        
        [JsonProperty]
        public string Signature { get; set; }
        
        [JsonProperty]
        public bool BypassRSAVerify { get; set; }
        
        [JsonProperty("Money")]
        public decimal Amount { get; set; }

        [JsonProperty("MoneyString")]
        public string AmountString { get; set; }
        
        [JsonProperty]
        public decimal Payout { get; set; }

        [JsonProperty("RoundNo")]
        public string RoundId { get; set; }

        [JsonProperty("TicketNo")]
        public string TransactionId { get; set; }

        [JsonProperty]
        public string Ip { get; set; }

        [JsonProperty]
        public int Type { get; set; }

        [JsonProperty]
        public string Currency { get; set; }

        [JsonProperty("GameType")]
        public string GameId { get; set; }

        [JsonProperty]
        public IDictionary<string, decimal> BetItem { get; set; }

        [JsonProperty]
        public IDictionary<string, decimal> PayoutDetail { get; set; }

        [JsonProperty("BetTime")]
        public string BetTimeString { get; set; }

        [JsonProperty("PayoutTime")]
        public string PayoutTimeString { get; set; }
        
        [JsonProperty]
        public decimal ValidBet { get; set; }

        [JsonProperty]
        public decimal TotalBet { get; set; }
        
        [JsonProperty]
        public string[] SourceResult { get; set; }
        
        [JsonProperty]
        public string[] Result { get; set; }
    }
}
