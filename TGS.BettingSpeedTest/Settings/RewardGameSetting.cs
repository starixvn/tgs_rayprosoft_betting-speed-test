namespace TGS.BettingSpeedTest.Settings
{
    public class RewardGameSetting
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:51672";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-reward-campaign-game-api.tgpasia.com";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://reward-campaign-game-api.tgpasia.com";
#endif

        public static class ApiPath
        {
            public static readonly string Draw = "/api/draw";
        }
    }
}