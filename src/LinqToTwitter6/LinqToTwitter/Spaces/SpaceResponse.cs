using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class SpaceResponse
    {
        [JsonPropertyName("data")]
        public MuteResponseData? Data { get; init; }
    }
}
