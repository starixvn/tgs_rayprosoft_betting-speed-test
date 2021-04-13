using System.Runtime.Serialization;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    [DataContract]
    public class JackpotContribution
    {
        [DataMember]
        public string jpid { get; set; }

        [DataMember]
        public string jpcur { get; set; }

        [DataMember]
        public decimal jprate { get; set; }

        [DataMember]
        public decimal jpamt { get; set; }

        [DataMember]
        public decimal cvtamt { get; set; }

        [DataMember]
        public decimal jpbal { get; set; }
    }
}
