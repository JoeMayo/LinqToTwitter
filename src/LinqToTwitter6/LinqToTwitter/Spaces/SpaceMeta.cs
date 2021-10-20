using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class SpaceMeta
    {
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }
    }
}