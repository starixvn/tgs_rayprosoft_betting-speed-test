using System;

namespace TGS.BettingSpeedTest.Messages.System
{
    public class JoinRewardCampaignRequest
    {
        public Guid RewardCampaignId { get; set; }
        public string PlayerToken { get; set; }
    }
}