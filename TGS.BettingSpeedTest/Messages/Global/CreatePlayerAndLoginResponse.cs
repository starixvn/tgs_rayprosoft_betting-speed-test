using System;
using System.Text.Json.Serialization;

namespace TGS.BettingSpeedTest.Messages.Global
{
    public record CreatePlayerAndLoginResponse
    {
        [JsonPropertyName("err")]
        public int Error { get; init; }

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("loginrecordid")]
        public Guid LoginRecordId { get; init; }

        [JsonPropertyName("encryptedtoken")]
        public string EncryptedToken { get; init; }
    }
}