// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class AuthorizePlayerRequest
    {
        public string ipaddress { get; set; }
        public string username { get; set; }
        public string userid { get; set; }
        public string lang { get; set; }
        public string cur { get; set; }
        public int betlimitid { get; set; }
        public bool istestplayer { get; set; }
        public int platformtype { get; set; }
        public int playertype => 4;
    }
}