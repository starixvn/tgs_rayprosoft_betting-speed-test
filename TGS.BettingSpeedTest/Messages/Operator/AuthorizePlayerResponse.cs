// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class AuthorizePlayerResponse
    {
        public string authtoken { get; set; }
        public bool isnew { get; set; }
        public string sunbetcredittoken { get; set; }
        public string userId { get; set; }
    }
}