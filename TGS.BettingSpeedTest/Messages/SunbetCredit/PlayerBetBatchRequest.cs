using System;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class PlayerBetBatchRequest
    {
        public DateTimeOffset? GameActionRecordedFrom { get; set; }
        public DateTimeOffset GameActionRecordedTo { get; set; }
        public string PlayerId { get; set; }
    }
}
