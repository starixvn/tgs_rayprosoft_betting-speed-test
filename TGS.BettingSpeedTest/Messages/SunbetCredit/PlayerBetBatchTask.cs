namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class PlayerBetBatchTask
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int ThreadId { get; set; }
        public long ExecutionTimeMs { get; set; }
    }
}
