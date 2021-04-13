using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.XPG
{
    public class TransactionRequest
    {
        [JsonProperty("Login")]
        public string PlayerCode { get; set; }

        [JsonProperty("Session")]
        public string PlayerToken { get; set; }

        public string GameId { get; set; }

        public string RoundId { get; set; }

        public decimal Amount { get; set; }

        public string CreditDetails { get; set; }

        public int OperatorId { get; set; }

        //It is empty when game type is settlement
        public string Sequence { get; set; }
    }
}