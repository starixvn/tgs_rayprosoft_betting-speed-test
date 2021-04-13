using System;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class DebitPlayerWalletRequest
    {
        public Guid PlayerId { get; set; }
        public string SunbetCreditToken { get; set; }
    }
}
