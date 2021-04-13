namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class GetSunbetCreditLobbyTokenResponse : SunbetCreditLobbyApiResponse
    {
        public string SunbetCreditToken { get; set; }
        public string PlayerToken { get; set; }
    }
}
