using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class PlaceAndSettleBetResponse : BaseResponse
    {
        [JsonProperty("retransmission")]
        public bool Retransmission { get; set; }
    }
}