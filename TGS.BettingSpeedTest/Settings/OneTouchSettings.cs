namespace TGS.BettingSpeedTest.Settings
{
    public static class OneTouchSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:64411/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-ot.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-wd.tgpaccess.com/api";
#endif

        public static readonly string TestGameId = "test";

        public static class OAuth
        {
            public static readonly string ClientId = "OneTouch-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "4yGrV9Kmp9Yzw9XF776Vt5FFsmzXWH0nIBF8fdkMa5Uo";
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