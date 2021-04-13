namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class AuthenticateManyRequest
    {
        public int player_id_from { get; set; }
        public int player_id_to { get; set; }
        public string clientIp { get; set; }
    }
}
