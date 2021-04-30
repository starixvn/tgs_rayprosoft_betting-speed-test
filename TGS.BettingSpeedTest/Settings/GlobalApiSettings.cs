namespace TGS.BettingSpeedTest.Settings
{
    public static class GlobalApiSettings
    {
#if LOCAL
        public static readonly string ApiEndpoint = "http://localhost:7002";
#endif

#if DEV
        public static readonly string ApiEndpoint = "http://qa-system.tgpaccess.com";
#endif

        public static class ApiPath
        {
            public static string CreateRewardCampaign = "/api/ugs/reward";
            public static string JoinRewardCampaign = "/api/ugs/reward/join";
        }
    }
}