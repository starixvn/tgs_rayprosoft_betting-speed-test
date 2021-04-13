namespace TGS.BettingSpeedTest.Settings
{
    public sealed class JDBSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:59060/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-jdb.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-jdb.tgpaccess.com/api";
#endif

        public static readonly string TestGameId = "JDB-FISH-001";
        public static readonly string GameType = "FH";
        public static readonly string AWCPlatform = "JDBFISH";
        public static readonly string Key = "URWhsMleKoo3deZrk0I";
    }
}
