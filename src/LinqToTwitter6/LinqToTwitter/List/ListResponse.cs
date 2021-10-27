using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class ListResponse
    {
        [JsonPropertyName("data")]
        public ListResponseData? Data { get; init; }
    }
}
