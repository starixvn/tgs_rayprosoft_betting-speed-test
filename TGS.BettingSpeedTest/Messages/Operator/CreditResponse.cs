// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class CreditResponse
    {
        public decimal bal { get; set; }
        public string cur { get; set; }
        public string txid { get; set; }
        public string ptxid { get; set; }
        public bool dup { get; set; }
    }
}