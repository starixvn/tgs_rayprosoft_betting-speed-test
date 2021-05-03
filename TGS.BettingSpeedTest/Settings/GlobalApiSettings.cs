namespace TGS.BettingSpeedTest.Settings
{
    public static class GlobalApiSettings
    {
#if LOCAL
        public static readonly string ApiEndpoint = "http://localhost:7003";
#endif

#if DEV
        public static readonly string ApiEndpoint = "https://dev.global.tgsdev.gq/";
#endif

        public static class ApiPath
        {
            public static string CreatePlayerAndLogin = "/global-api/brand/player/createandlogin";
        }
    }
}