namespace TGS.BettingSpeedTest.Settings
{
    public static class GamingArcadeAsiaSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:42000/api/gamingarcadeasia";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-ga.tgpaccess.com/api/gamingarcadeasia";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp.tgpaccess.com/api/gamingarcadeasia";
#endif

        public static readonly string TestSlotGameId = "fairy";

        public static class OAuth
        {
            public static readonly string ClientId = "GamingArcadeAsia-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "advWNFg8X6eKc1pavZGUWBMv3lNxQruDUxVoFSkdanc";
#endif

#if PRODUCTION
            public static readonly string ClientSecret = "4gmhLXQB6veP4K3kVHkeS3JRXvm7H6gjyRQmhY3z25FU";
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
            public static readonly string BetsSettle = "/bets/settle";
        }
    }
}