namespace TGS.BettingSpeedTest.Settings
{
    public static class SboSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:62330/api/provider";
#endif

#if DEV
        public static string ApiEndpoint = "{0}://qa-gp-ag.tgpaccess.com/api";
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