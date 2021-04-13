using System;
using System.Xml.Serialization;

namespace TGS.BettingSpeedTest.Messages.AsiaGaming
{
    [Serializable]
    [XmlRoot(ElementName = "Data")]
    public class PlaceBetRequest : BetRequest
    {
        [XmlElement(ElementName = "Record")]
        public PlaceBetRecord Record { get; set; }
    }

    public class PlaceBetRecord : Record
    {
        [XmlElement(ElementName = "value")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "betTime")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "platformType")]
        public string GamePlatformType { get; set; }

        [XmlElement(ElementName = "transactionID")]
        public string TransactionId { get; set; }

        [XmlElement(ElementName = "round")]
        public string GameRoomType { get; set; }

        [XmlElement(ElementName = "playtype")]
        public string BetType { get; set; }
    }
}