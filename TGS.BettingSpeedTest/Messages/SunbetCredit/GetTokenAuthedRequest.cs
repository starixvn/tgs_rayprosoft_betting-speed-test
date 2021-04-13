namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class GetTokenAuthedRequest
    {
        public string SessionId { get; set; }

        public int SiteId { get; set; }

        public string AcctId { get; set; }

        public string AcctLogin { get; set; }

        public string Lang { get; set; }

        public string ClientIpAddress { get; set; }
        public byte PlatformType { get; set; }

        public string GameCode { get; set; }
    }
}
