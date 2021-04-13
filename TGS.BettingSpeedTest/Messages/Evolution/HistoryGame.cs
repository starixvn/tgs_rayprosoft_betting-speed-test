using System;
using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class HistoryGame
    {
        [JsonProperty("id")]
        public string RoundId { get; set; }
        
        [JsonProperty("startedAt")]
        public DateTimeOffset CreatedOn { get; set; }
        
        [JsonProperty("settledAt")]
        public DateTimeOffset ClosedOn { get; set; }

        public string Status = "Resolved";
        
        public string GameType { get; set; }
        
        public GameTable Table { get; set; }
        
        [JsonProperty("wager")]
        public decimal TotalBetAmount { get; set; }
        
        [JsonProperty("payout")]
        public decimal TotalWinAmount { get; set; }
        
        public Participant[] Participants { get; set; }
        
        [JsonProperty("result")]
        public dynamic GameResult { get; set; }
    }

    public class Participant
    {
        public Guid PlayerId { get; set; }
        
        [JsonProperty("sessionId")]
        public string EvolutionSessionId { get; set; }
        
        [JsonProperty("casinoSessionId")]
        public string PlayerToken { get; set; }
        
        [JsonProperty("bets")]
        public BetDetail[] BetDetails { get; set; }
        
        public string Currency { get; set; }
    }

    public class BetDetail
    {
        public string TransactionId { get; set; }
        
        [JsonProperty("code")]
        public string BetType { get; set; }
        
        [JsonProperty("stake")]
        public decimal BetAmount { get; set; }
        
        [JsonProperty("payout")]
        public decimal WinAmount { get; set; }
        
        [JsonProperty("placedOn")]
        public DateTimeOffset BetOn { get; set; }
    }
}