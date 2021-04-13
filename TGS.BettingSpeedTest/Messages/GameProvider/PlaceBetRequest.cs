// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class PlaceBetRequest
    {
        public string authtoken { get; set; }
        public List<TransactionRequest> transactions { get; set; }

        public class TransactionRequest
        {
            public decimal amt { get; set; }
            public string cur { get; set; }
            public bool uselockedamt { get; set; }
            public string txid { get; set; }
            public string txrefid { get; set; }
            public string roundid { get; set; }
            public string betrecid { get; set; }
            public string gameid { get; set; }
            public string timestamp { get; set; }
            public string desc { get; set; }
        }
    }
}