using CommandLine;

namespace TGS.BettingSpeedTest
{
    internal class Options
    {
        [Option('p', "players", Default = 25)]
        public int NumberOfPlayers { get; set; }

        [Option('j', "jackpot", Default = false, HelpText = "enable jackpot.")]
        public bool EnableJackpot { get; set; }

        [Option('s', "scenario", Default = "PlaceAndSettle", HelpText = "scenario to run.")]
        public string Scenario { get; set; }

        [Option('h', "https?", Default = false, HelpText = "enable https")]
        public bool EnableHttps { get; set; }

        [Option('g', "gp-code", Default = "TGP", HelpText = "test gp")]
        public string GameProviderCode { get; set; }

        [Option('b', "brand-code", Default = "GPI", HelpText = "test brand")]
        public string BrandCode { get; set; }

        [Option('t', "reward-type", Default = "2", HelpText = "reward campaign restriction type")]
        public string RewardCampaignRestrictionType { get; set; }

        [Option('w', "chance-to-win", Default = "50", HelpText = "chance to win")]
        public string ChanceToWin { get; set; }

        [Option('d', "MillisecondsDelay", Default = 300, HelpText = "MillisecondsDelay")]
        public int MillisecondsDelay { get; set; }

        [Option('r', "PercentageOfPlay", Default = (byte)100, HelpText = "PercentageOfPlay")]
        public byte PercentageOfPlay { get; set; }

        [Option('c', "PlayCycleOfPercentage", Default = (byte)1, HelpText = "PlayCycleOfPercentage")]
        public byte PlayCycleOfPercentage { get; set; }
    }
}