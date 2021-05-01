namespace TGS.BettingSpeedTest.Settings
{
    public static class AsiaGamingSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:43000/api";
#endif

#if DEV
    public static string ApiEndpoint = "{0}://qa-gp-ag.tgpaccess.com/api";
#endif

        public const string AgentCode = "FK5";
        public const string GamePlatformType = "AGIN";
        public const string GameRoomType = "DSP";
        public const string GameType = "BAC";
        public const string SuccessfulErrorCode = "OK";
        public const decimal BetAmount = 1;
        public const string TimestampFormat = "MM/dd/yyyy HH:mm:sss";
        public const string Currency = "CNY";
        public const string Bet = "bet";
        public const string Win = "win";
        public const string Lose = "lose";
        public const string Platform = "DESKTOP";

        public static class ApiPath
        {
            public static readonly string BetsPlaceAndSettle = "/bets/placeandsettle";
        }
    }
}