namespace TGS.BettingSpeedTest.Settings
{
    public static class TycheSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:54326/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-ty.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-ty.tgpaccess.com/api";
#endif
        public static readonly string Authorization = "Bearer Tyche-UGS:aLhoFFh3aAKK5qUGEZzSUlzeuf9IGJBT4Pj8DaPGSOfi";
    }
}