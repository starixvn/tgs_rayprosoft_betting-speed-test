namespace TGS.BettingSpeedTest.Settings
{
    public static class DreamTechSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:52607/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-dt.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-dt.tgpaccess.com/api";
#endif

        public static readonly string Business = "NNTI_TESTS_TGPTEST3";
    }
}