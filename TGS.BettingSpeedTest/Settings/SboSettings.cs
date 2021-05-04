namespace TGS.BettingSpeedTest.Settings
{
    public static class SboSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:62330/api/provider";
#endif

#if DEV
        public static string ApiEndpoint = "https://dev.game-sbo.tgsdev.gq/api/provider";
#endif

#if STAGING
        public static string ApiEndpoint = "https://game-sbo.tgsstg.gq/api/provider";
#endif

#if PROD
        public static string ApiEndpoint = "https://game-sbo.yydsgo.xyz/api/provider";
#endif

        public const string CompanyKey = "A33F1A3F3BC8485BA242F9EE61321E90";
        public const int ProductType = 1;
        public const int GameType = 1;

        public static class ApiPath
        {
            public static readonly string PlaceBet = "/deduct";
            public static readonly string SettleBet = "/settle";
        }
    }
}