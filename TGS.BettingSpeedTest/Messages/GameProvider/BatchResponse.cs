using System;
using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class BatchResponse
    {
        public int err { get; set; }
        public string errdesc { get; set; }
        public string batchid { get; set; }
        public bool dup { get; set; }
        public DateTimeOffset batchtime { get; set; }
        public int transcount { get; set; }
        public Dictionary<Guid, decimal> playerbalances { get; set; }
        public BatchResponseTransaction[] errors { get; set; }

        public class BatchResponseTransaction
        {
            public Guid userid { get; set; }
            public string betrecid { get; set; }
            public string txid { get; set; }
            public Guid? ptxid { get; set; }
            public bool dup { get; set; }
            public int err { get; set; }
            public string errdesc { get; set; }
        }
    }
}
