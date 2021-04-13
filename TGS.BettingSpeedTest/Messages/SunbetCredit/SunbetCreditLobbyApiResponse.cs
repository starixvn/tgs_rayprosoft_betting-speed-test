namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class SunbetCreditLobbyApiResponse
    {
        public SunbetCreditLobbyApiResponse()
        {
            err = ApiResponseCode.Failed;
        }
        public ApiResponseCode err { get; set; }
        public string errdesc { get; set; }
    }
}
