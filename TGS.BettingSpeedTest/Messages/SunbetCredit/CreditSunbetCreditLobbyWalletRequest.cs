using System;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class CreditSunbetCreditLobbyWalletRequest
    {
        public Guid PlayerId { get; set; }
        public string SunbetCreditToken { get; set; }
        public decimal Amount { get; set; }
    }
}
