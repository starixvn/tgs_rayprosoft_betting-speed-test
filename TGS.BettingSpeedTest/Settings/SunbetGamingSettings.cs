namespace TGS.BettingSpeedTest.Settings
{
    public static class SunbetGamingSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:56399/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-sbg.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-sbg.tgpaccess.com/api";
#endif

        public static readonly string TestGameId = "zhajinhua";
        public static string Currency = "RMB";

        public static class OAuth
        {
            public static readonly string ClientId = "SunbetGaming-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "YQTiai1rmm6Ey5lmtRE1S0E5hRhc6O56LOVsPEncrmdp";
#endif

#if PRODUCTION
            public static readonly string ClientSecret = "";
#endif
            public static readonly string GrantType = "client_credentials";

            public static readonly string Scope = "bets";
            public static readonly int TokenExpiresIn = 3600;
            public static readonly string TokenType = "Bearer";
        }

        public static class ApiPath
        {
            public static readonly string OAuthToken = "/oauth/token";
            public static readonly string LockTransaction = "/transactions/lock";
            public static readonly string BetsPlace = "/bets/place";
            public static readonly string BetsSettle = "/bets/settle";
        }
    }
}