﻿using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class GameTable
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}