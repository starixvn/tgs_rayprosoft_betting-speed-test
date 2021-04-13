using System.Xml.Serialization;

namespace TGS.BettingSpeedTest.Messages.AsiaGaming
{
    public class BetRequest
    {
        [XmlElement(ElementName = "agentCode")]
        public string PlayerCode { get; set; }
    }

    public class Record
    {
        [XmlElement(ElementName = "sessionToken")]
        public string TokenId { get; set; }

        [XmlElement(ElementName = "currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "playname")]
        public string PlayerCode { get; set; }

        [XmlElement(ElementName = "agentCode")]
        public string AgentCode { get; set; }

        [XmlElement(ElementName = "gametype")]
        public string GameType { get; set; }

        [XmlElement(ElementName = "tableCode")]
        public string TableCode { get; set; }

        [XmlElement(ElementName = "gameCode")]
        public string RoundId { get; set; }

        [XmlElement(ElementName = "transactionType")]
        public string TransactionType { get; set; }

        [XmlElement(ElementName = "transactionCode")]
        public string TransactionCode { get; set; }

        [XmlElement(ElementName = "deviceType")]
        public string Platform { get; set; }
    }
}