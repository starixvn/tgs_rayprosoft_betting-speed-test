using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class TransferResponse
    {
        [JsonProperty("result")]
        public ResultResponse Result { get; set; }

        public class ResultResponse : CommonResultResponse
        {
            [JsonProperty("cash")]
            public decimal Cash { get; set; }

            [JsonProperty("bonus")]
            public decimal Bonus { get; set; }

            [JsonProperty("externalTransactionId")]
            public string TransactionId { get; set; }

            [JsonProperty("externalTransactionTimeStamp")]
            public string TimeStamp { get; set; }

            [JsonProperty("conversionRate")]
            public string ConversionRate { get; } = "1";
        }
    }
}
