namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class GetPlayerInfoResponse
    {
        public string acct_id { get; set; }
        public string acct_login { get; set; }
        public string parent_id { get; set; }
        public string parent_login { get; set; }
        public string level { get; set; }
        public string currency { get; set; }
        public decimal main_balance { get; set; }
        public decimal? wallet_balance { get; set; }
        public decimal? in_amount { get; set; }
        public decimal? out_amount { get; set; }
        public decimal? win_amount { get; set; }
        public string acct_status { get; set; }
        public string bet_status { get; set; }
        public bool is_test_player { get; set; }
        public int bet_limit_id { get; set; }
    }
}
