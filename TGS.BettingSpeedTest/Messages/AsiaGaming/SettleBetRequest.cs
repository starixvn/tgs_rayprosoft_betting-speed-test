using System;
using System.Xml.Serialization;

namespace TGS.BettingSpeedTest.Messages.AsiaGaming
{
    [Serializable]
    [XmlRoot(ElementName = "Data")]
    public class SettleBetRequest : BetRequest
    {
        [XmlElement(ElementName = "Record")]
        public SettleBetRecord Record { get; set; }
    }

    public class SettleBetRecord : Record
    {
        [XmlElement(ElementName = "netAmount")]
        public decimal WinAmount { get; set; }

        [XmlElement(ElementName = "validBetAmount")]
        public decimal BetAmount { get; set; }

        [XmlElement(ElementName = "billNo")]
        public string BetId { get; set; }

        [XmlElement(ElementName = "transactionID")]
        public string[] TransactionIds { get; set; }

        [XmlElement(ElementName = "settletime")]
        public string Timestamp { get; set; }

        [XmlElement(ElementName = "ticketStatus")]
        public string Result { get; set; }

        [XmlElement(ElementName = "gameResult")]
        public string GameResult { get; set; }

        [XmlElement(ElementName = "finish")]
        public string RoundClosed { get; set; }
    }
}