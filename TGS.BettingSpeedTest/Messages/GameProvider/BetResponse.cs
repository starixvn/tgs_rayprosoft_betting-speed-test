// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class BetResponse
    {
        public int err { get; set; }
        public string errdesc { get; set; }
        public decimal bal { get; set; }
        public string cur { get; set; }
        public List<TransactionResponse> transactions { get; set; }

        public class TransactionResponse
        {
            public string txid { get; set; }
            public Guid ptxid { get; set; }
            public bool dup { get; set; }
        }
    }
}