// ReSharper disable InconsistentNaming

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class SeamlessProductsRequest
    {
        public string authtoken { get; set; }
        public string ipaddress { get; set; }
        public string lobbypath { get; set; }
        public bool fromugslobby { get; set; }
    }
}