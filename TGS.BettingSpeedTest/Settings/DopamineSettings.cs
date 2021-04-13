namespace TGS.BettingSpeedTest.Settings
{
    public static class DopamineSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:60460/api/dopamine";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-tgp.tgpaccess.com/api/dopamine";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp.tgpaccess.com/api/dopamine";
#endif

        public static readonly string TestSlotGameId = "DragonsLuck";
        public static readonly string TestTableGameId = "PuntoBanco";

        public static class OAuth
        {
            public static readonly string ClientId = "Dopamine-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "SBX0TtQwLMyIYRoYKbLfNTxH7OOgffEtA0ymrFChGYowSUyfAAecSC";
#endif

#if PRODUCTION
            public static readonly string ClientSecret = "otN6wLDZlBbkk9dunVhKvKyeeIFEAz3TzcXJwNAFESKo";
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