using CommandLine;

namespace TGS.BettingSpeedTest
{
    internal record Options
    {
        [Option('p', "players", Default = 25)]
        public int NumberOfPlayers { get; init; }

        [Option('s', "scenario", Default = "PlaceAndSettle", HelpText = "scenario to run.")]
        public string Scenario { get; init; }

        [Option('h', "https?", Default = false, HelpText = "enable https")]
        public bool EnableHttps { get; init; }

        [Option('g', "gp-code", Default = "SBO", HelpText = "test gp")]
        public string GameProviderCode { get; init; }

        [Option('b', "brand-code", Default = "GPI", HelpText = "test brand")]
        public string BrandCode { get; init; }

        [Option('d', "MillisecondsDelay", Default = 300, HelpText = "MillisecondsDelay")]
        public int MillisecondsDelay { get; init; }

        [Option('r', "PercentageOfPlay", Default = (byte) 100, HelpText = "PercentageOfPlay")]
        public byte PercentageOfPlay { get; init; }

        [Option('c', "PlayCycleOfPercentage", Default = (byte) 1, HelpText = "PlayCycleOfPercentage")]
        public byte PlayCycleOfPercentage { get; init; }
    }
}