using System;
using System.Collections.Generic;
using System.Linq;

namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class PlayerBetBatchResponse : SunbetCreditLobbyApiResponse
    {
        public PlayerBetBatchResponse()
        {
            Data = new List<PlayerBetBatchTask>();
        }

        public int Total => Data.Count;

        public int Success
        {
            get { return Data.Count(d => d.Succeeded); }
        }

        public int Failure
        {
            get { return Data.Count(d => !d.Succeeded); }
        }

        public List<PlayerBetBatchTask> Data { get; set; }
        public DateTimeOffset? GameActionRecordedFrom { get; set; }
        public DateTimeOffset GameActionRecordedTo { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalBatches { get; set; }
        public int TotalGameActions { get; set; }
        public int ThreadId { get; set; }
    }
}
