namespace TGS.BettingSpeedTest.Settings
{
    public static class XPGSettings
    {
#if LOCAL
        public static string ApiEndpoint = "{0}://localhost:59975/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-xpg.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-xpg.tgpaccess.com/api";
#endif


        public static readonly string TestGameId = "10"; //Blackjack
        public static readonly int OperatorId = 1064;
        public static readonly string DebitDetail = "BJ;{0}:1.00;1;0";
        public static readonly string CreditDetail = "BJ;2,13,35:12$65317578,15$65317578,32$65317578,40$65317579,49$65317579;62697114,1,65317578,250.00,{0}.00";
        public static readonly string CreditDetailWithMultiBets = "BJ;2,13,35:12$65317578,15$65317578,32$65317578,40$65317579,49$65317579;62697114,0,65317578,1.00,{0}.00:62697115,1,65317578,1.00,0.00:62697115,2,65317578,1.00,0.00";

        public static class ApiPath
        {
            public static readonly string BetsPlace = "/Debit";
            public static readonly string BetsSettle = "/Credit";
        }
    }
}