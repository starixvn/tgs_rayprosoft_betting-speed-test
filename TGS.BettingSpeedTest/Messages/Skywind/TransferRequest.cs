using System;

namespace TGS.BettingSpeedTest.Messages.Skywind
{
    public class TransferRequest : ApiRequest
    {
        public Guid Cust_id { get; set; }

        public string Cust_session_id { get; set; }

        public decimal Amount { get; set; }

        public string Currency_code { get; set; }

        public string Game_code { get; set; }

        public string Trx_id { get; set; }

        //round id
        public string Game_id { get; set; }

        public string Event_type { get; set; }

        //a spin sequence number
        public string Event_id { get; set; }

        public long Timestamp { get; set; }

        public string Game_type { get; set; }

        public string Platform { get; set; }
    }
}
