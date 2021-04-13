namespace TGS.BettingSpeedTest.Settings
{
    public static class EvolutionSettings
    {
#if LOCAL
        public static string ApiEndpoint = "http://localhost:5002/api";
#endif

#if QA
        public static string ApiEndpoint = "{0}://qa-gp-evo.tgpaccess.com/api";
#endif

#if PRODUCTION
        public static string ApiEndpoint = "{0}://perf-test-gp-evo.tgpaccess.com/api";
#endif

        public static readonly string TestGameType = "baccarat";
        public static readonly string TestTableId = "oytmvb9m1zysmc44";
        public static string Currency = "CNY";
        public const decimal BetAmount = 1;
        public const string SuccessfulErrorCode = "OK";

        public static class ApiPath
        {
            public static readonly string Debit = "/debit";
            public static readonly string BetsPlaceAndSettle = "/bets/placeandsettle";
        }
    }
}