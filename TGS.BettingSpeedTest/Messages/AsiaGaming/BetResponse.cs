using System.Xml.Serialization;

namespace TGS.BettingSpeedTest.Messages.AsiaGaming
{
    [XmlRoot(ElementName = "TransferResponse")]
    public class BetResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string ErrorCode { get; set; }

        [XmlElement]
        public decimal Balance { get; set; }
    }
}