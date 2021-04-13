namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class DebitSunbetCreditLobbyWalletResponse : SunbetCreditLobbyApiResponse
    {
        public string UgsBalance { get; set; }
        public decimal SunbetCreditBalance { get; set; }
        public string Currency { get; set; }
        public string Txid { get; set; }
    }
}
