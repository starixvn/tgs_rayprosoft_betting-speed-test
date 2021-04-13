// ReSharper disable InconsistentNaming

using System;

namespace TGS.BettingSpeedTest.Messages.Operator
{
    public class PlayerDataResponse
    {
        public Guid playerid { get; set; }
        public string playercode { get; set; }
        public Guid tokenid { get; set; }
        public string encryptedtokenid { get; set; }
    }
}