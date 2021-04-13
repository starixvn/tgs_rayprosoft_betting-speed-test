namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public enum ApiResponseCode
    {
        Succeeded = 0,
        SessionExpired = 1001,
        Failed = 900,
        DuplicateBet = 751,
        InvalidTransactionDate = 706,
        InvalidSessionToken = 701,
        InvalidIpAddress = 708,
        InvalidCurrency = 703,
        Mismatched = 724,
        InvalidTgpBalance = 713
    }
}
