// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class CreditRequest
    {
        public string userid { get; set; }
        public decimal amt { get; set; }
        public string cur { get; set; }
        public string txid { get; set; }
        public string timestamp { get; set; }
    }
}