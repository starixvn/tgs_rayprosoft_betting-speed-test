// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class SettleBetRequest : SessionBasedRequest
    {
        public List<TransactionRequest> transactions { get; set; }

        public decimal unlockedamt { get; set; }

        public class TransactionRequest
        {
            public decimal amt { get; set; }
            public string cur { get; set; }
            public string txid { get; set; }
            public int txtype { get; set; }
            public string txrefid { get; set; }
            public bool? roundclosed { get; set; }
            public string roundid { get; set; }
            public string betrecid { get; set; }
            public string timestamp { get; set; }
            public string desc { get; set; }
            public JackpotContribution[] jpcontribs { get; set; }
        }
    }
}