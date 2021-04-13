// ReSharper disable InconsistentNaming

using System;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class LockTransactionResponse
    {
        public int err { get; set; }
        public string errdesc { get; set; }
        public decimal bal { get; set; }
        public string cur { get; set; }
        public string txid { get; set; }
        public Guid ptxid { get; set; }
        public bool dup { get; set; }
    }
}