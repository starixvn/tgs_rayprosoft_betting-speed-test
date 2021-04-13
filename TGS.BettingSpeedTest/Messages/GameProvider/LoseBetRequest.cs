// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class LoseBetRequest : SessionBasedRequest
    {
        public List<TransactionRequest> transactions { get; set; }

        public class TransactionRequest
        {
            public string cur { get; set; }
            public string txid { get; set; }
            public string txrefid { get; set; }
            public string roundid { get; set; }
            public bool? roundclosed { get; set; }
            public string betrecid { get; set; }
            public string timestamp { get; set; }
            public string desc { get; set; }
            public JackpotContribution[] jpcontribs { get; set; }
        }
    }
}