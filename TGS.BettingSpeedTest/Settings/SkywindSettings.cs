namespace TGS.BettingSpeedTest.Settings
{
    public static class SkywindSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:53977/api";
        public static readonly string MerchantCode = "rgnzmfffsutph";
        public static readonly string MerchantPassword = "l9XgoRxn";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-sw.tgpaccess.com/api";
        public static readonly string MerchantCode = "kicfmjlsqbwrr";
        public static readonly string MerchantPassword = "oJpAr6nU";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-sw.tgpaccess.com/api";
        public static readonly string MerchantCode = "";
        public static readonly string MerchantPassword = "";
#endif

        public static class ApiPath
        {
            public static readonly string BetsPlace = "/debit";
            public static readonly string BetsSettle = "/credit";
        }
    }
}