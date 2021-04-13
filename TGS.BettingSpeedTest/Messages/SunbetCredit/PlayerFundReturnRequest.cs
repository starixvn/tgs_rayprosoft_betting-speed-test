using System;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class PlayerFundReturnRequest
    {
        public string player_id { get; set; }
        public string session_token { get; set; }
        public string ext_tran_id { get; set; }
        public DateTime tran_date { get; set; }
        public decimal amount { get; set; }
        public decimal out_hist_amount { get; set; }
        public decimal remain_balance { get; set; }
    }
}
