using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class MuteMeta
    {
        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }
    }
}