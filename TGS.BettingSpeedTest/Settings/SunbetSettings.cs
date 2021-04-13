namespace TGS.BettingSpeedTest.Settings
{
    public static class SunbetSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:57313/api/sunbet";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-sb.tgpaccess.com/api/sunbet";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://gp-sb.tgpaccess.com/api/sunbet";
#endif

        public static readonly string TestGameId = "1";

        public static class OAuth
        {
            public static readonly string ClientId = "Sunbet-UGS";
#if LOCAL || QA
            public static readonly string ClientSecret = "5AzhXf5bMTHWVOJyQxkkhpz0fP5TvdUv2JEju6ipT0LHAZzVQLhSZb";
            public static readonly string SecurityKey = "9VBlIs9mF2zAxZCnrThwLG7MEaWvhZDMHcU39PBdIA2";
#endif

#if PRODUCTION
            //This is not correct
            public static readonly string ClientSecret = "5AzhXf5bMTHWVOJyQxkkhpz0fP5TvdUv2JEju6ipT0LHAZzVQLhSZb";
            public static readonly string SecurityKey = "9VBlIs9mF2zAxZCnrThwLG7MEaWvhZDMHcU39PBdIA2";
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
            public static readonly string BetsSettle = "/batch/bets/settle";
        }
    }
}