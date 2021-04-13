namespace TGS.BettingSpeedTest.Messages.GameProvider
{
    public class RewardCampaignGameResponse
    {
        public string code { get; set; }
        public string currency { get; set; }
        public decimal bonus { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public int usedchances { get; set; }
        public int prizeorder { get; set; }
        public bool wageringactive { get; set; }
        public WageringRewardCampaign wageringcampaign { get; set; }
    }

    public class WageringRewardCampaign
    {
        public string campaignCode { get; set; }
        public string campaignName { get; set; }
        public decimal wageringAmount { get; set; }
        public decimal turnover { get; set; }
    }
}