using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Tyche
{
    public class TransferRequest : ApiRequest
    {
        [JsonProperty("params")]
        public DataRequest Data { get; set; }

        public class DataRequest : GameDataRequest
        {
            [JsonProperty("amount")]
            public decimal Amount { get; set; }

            [JsonProperty("transactionTimeStamp")]
            public string TimeStamp { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("bonusBalance")]
            public string BonusBalance { get; set; }

            [JsonProperty("walletStrategy")]
            public string WalletStrategy { get; set; }

            [JsonProperty("currency")]
            public string Currency { get; set; }

            [JsonProperty("bonusConversion")]
            public bool BonusConversion { get; set; }

            [JsonProperty("freeSpinWin")]
            public bool FreeSpinWin { get; set; }
        }
    }
}
