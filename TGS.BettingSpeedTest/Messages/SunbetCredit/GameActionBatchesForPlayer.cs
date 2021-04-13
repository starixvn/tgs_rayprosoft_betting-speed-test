using System;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class GameActionBatchesForPlayer
    {
        public long Id { get; set; }
        public string BetId { get; set; }
        public Guid PlayerId { get; set; }
        public decimal TotalBetAmount { get; set; }
        public decimal TotalWinAmount { get; set; }
        public decimal Balance { get; set; }
        public int GameTypeCode { get; set; }
        public string RawToken { get; set; }
        public string ExternalTransactionId { get; set; }
        public decimal TransactionAmount { get; set; }
        public bool IsSynced { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? SyncedOn { get; set; }
    }
}
