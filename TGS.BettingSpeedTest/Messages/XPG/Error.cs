namespace TGS.BettingSpeedTest.Messages.XPG
{
    public class Error
    {
        public int ErrorCode { get; set; }

        public bool HasErrors { get; set; }

        public string Message { get; set; }
    }
}