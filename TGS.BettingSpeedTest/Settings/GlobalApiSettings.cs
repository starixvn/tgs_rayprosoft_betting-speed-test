namespace TGS.BettingSpeedTest.Settings
{
    public static class GlobalApiSettings
    {
#if LOCAL
        public static readonly string ApiEndpoint = "https://localhost:7002";
#endif

#if DEV
        public static readonly string ApiEndpoint = "https://dev.global-api.tgsdev.gq/";
#endif

#if STAGING
        public static readonly string ApiEndpoint = "https://global-api.tgsstg.gq/";
#endif

#if PROD
        public static readonly string ApiEndpoint = "https://global-api.yydsgo.xyz/";
#endif

        public static class ApiPath
        {
            public static string CreatePlayerAndLogin = "/global-api/brand/player/createandlogin";
        }
    }
}