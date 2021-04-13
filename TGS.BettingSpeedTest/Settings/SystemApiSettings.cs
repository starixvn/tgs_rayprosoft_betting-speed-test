namespace TGS.BettingSpeedTest.Settings
{
    public static class SystemApiSettings
    {
#if LOCAL
        public static readonly string ApiEndpoint = "http://localhost:37169";
#endif

#if QA
        public static readonly string ApiEndpoint = "http://qa-system.tgpaccess.com";
#endif

#if PRODUCTION
        public static readonly string ApiEndpoint = "http://system.tgpaccess.com";
#endif

        public static class ApiPath
        {
            public static string CreateRewardCampaign = "/api/ugs/reward";
            public static string JoinRewardCampaign = "/api/ugs/reward/join";
        }
    }
}