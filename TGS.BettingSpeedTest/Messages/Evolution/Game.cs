﻿using Newtonsoft.Json;
using TGS.BettingSpeedTest.Messages.Evolution;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class Game
    {
        [JsonProperty("id")]
        public string RoundId { get; set; }
        
        public string Type { get; set; }

        public GameDetails Details { get; set; }
    }

    public class GameDetails
    {
        public GameTable Table { get; set; }
    }
}