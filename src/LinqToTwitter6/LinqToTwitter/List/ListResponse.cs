using LinqToTwitter.Common;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class ListResponse
    {
        [JsonPropertyName("data")]
        public ListResponseData? Data { get; init; }

        [JsonPropertyName("includes")]
        public TwitterInclude? Includes { get; set; }
    }
}
