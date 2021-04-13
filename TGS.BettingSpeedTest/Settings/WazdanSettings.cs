namespace TGS.BettingSpeedTest.Settings
{
    public static class WazdanSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:51082/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-wd.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-wd.tgpaccess.com/api";
#endif

        public static readonly string TestGameId = "65888";

        public static class OAuth
        {
            public static readonly string ClientId = "Wazdan-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "tvZbbmEb2nyqxnYKtAgN0f9v20JW7C1AFOhf0uoyuOT";
#endif

#if PRODUCTION
            public static readonly string ClientSecret = "RyhAnJtb63sgckC6tsYUN586AbAn8ETTHY6eJJzb4w2C";
#endif
            public static readonly string GrantType = "client_credentials";

            public static readonly string Scope = "bets";
            public static readonly int TokenExpiresIn = 3600;
            public static readonly string TokenType = "Bearer";
        }

        public static class ApiPath
        {
            public static readonly string OAuthToken = "/oauth/token";
            public static readonly string BetsPlace = "/bets/place";
            public static readonly string BetsLose = "/bets/lose";
            public static readonly string BetsSettle = "/bets/settle";
        }
    }
}