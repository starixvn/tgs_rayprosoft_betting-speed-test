// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages
{
    public class GetAccessTokenRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string scope { get; set; }
    }
}