namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class LockTransactionRequest : SessionBasedRequest
    {
        public decimal amt { get; set; }
        public string cur { get; set; }
        public string txid { get; set; }
        public string txrefid { get; set; }
        public string roundid { get; set; }
        public string gameid { get; set; }
        public string timestamp { get; set; }
    }
}