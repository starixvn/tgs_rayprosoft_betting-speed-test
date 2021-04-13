﻿using Newtonsoft.Json;

namespace TGS.BettingSpeedTest.Messages.Evolution
{
    public class TransactionRequest : BaseRequest
    {
        public Game Game { get; set; }

        public Transaction Transaction { get; set; }
    }
    
    public class Transaction
    {
        [JsonProperty("id")]
        public string EvolutionTransactionId { get; set; }
     
        [JsonProperty("refId")]   
        public string Id { get; set; }

        public decimal Amount { get; set; }
    }
}