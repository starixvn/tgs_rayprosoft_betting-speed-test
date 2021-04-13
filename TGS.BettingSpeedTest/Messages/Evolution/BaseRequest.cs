﻿using System;
using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class BaseRequest : Base
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        
        [JsonProperty("sid")]
        public string Token { get; set; }
        
        public string Currency { get; set; }
    }
}