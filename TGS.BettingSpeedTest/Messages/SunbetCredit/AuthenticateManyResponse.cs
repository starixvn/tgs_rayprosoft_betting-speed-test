namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class AuthenticateManyResponse
    {
        public int SiteId { get; set; }
        public string BrandId { get; set; }
        public int PlayerStatus { get; set; }
        public bool ShouldChangePassword { get; set; }
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string OldSessionId { get; set; }
        public string AcctId { get; set; }
        public string UserId { get; set; }
        public string SystemId { get; set; }
    }
}
