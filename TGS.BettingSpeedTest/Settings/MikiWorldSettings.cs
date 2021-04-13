namespace TGS.BettingSpeedTest.Settings
{
    public static class MikiWorldSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:5000/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-mw.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-mw.tgpaccess.com/api";
#endif

        public const int ChannelId = -1;
        public const string GameId = "100";
        public const string Event = "IncreaseCredit";
        public const decimal BetAmount = 1;
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        public const string Currency = "156";
        public const int Bet = 0;
        public const int Settle = 1;
        public const int SuccessfulErrorCode = 200;

        public static class ApiPath
        {
            public static readonly string BetsPlaceAndSettle = "/increaseCredit";
        }
    }
}