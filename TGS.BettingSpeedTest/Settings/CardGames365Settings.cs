namespace TGS.BettingSpeedTest.Settings
{
    public static class CardGames365Settings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:55001/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-365.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-365.tgpaccess.com/api";
#endif

        public static readonly string TestGameId = "560";
        public static string Currency = "3";

        public static class OAuth
        {
            public static readonly string ClientId = "365CardGames-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "4JMeW49e77TyE5Lxa4gKQgJ9jHdydWYm9cURrkJyB5NB";
#endif

#if PRODUCTION
            public static readonly string ClientSecret = "svFLThYCB2BCkfhHjW5szjA2rkPTaD3UsarsBE5CJsd6";
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