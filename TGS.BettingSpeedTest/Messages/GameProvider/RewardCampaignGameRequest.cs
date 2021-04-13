using System;

namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class RewardCampaignGameRequest
    {
        public string authtoken { get; set; }
        public Guid campaignid { get; set; }
    }
}