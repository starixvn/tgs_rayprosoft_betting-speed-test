using System;
using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class BatchRequest
    {
        public string batchid { get; set; }
        public string securitykey { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public List<BatchRequestTransaction> transactions { get; set; }

        public struct BatchRequestTransaction
        {
            public Guid userid { get; set; }
            public string tag { get; set; }
            public decimal amt { get; set; }
            public string cur { get; set; }
            public string txid { get; set; }
            public int? txtype { get; set; }
            public string txrefid { get; set; }
            public string betrecid { get; set; }
            public string gameid { get; set; }
            public string roundid { get; set; }
            public string roundresult { get; set; }
            public bool? roundclosed { get; set; }
            public string desc { get; set; }
        }
    }
}
