using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class BlockingResponse
    {
        [JsonPropertyName("data")]
        public BlockingResponseData? Data { get; init; }
    }
}
