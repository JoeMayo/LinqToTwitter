using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class MuteResponse
    {
        [JsonPropertyName("data")]
        public MuteResponseData? Data { get; init; }
    }
}
