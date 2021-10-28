using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record ListFollowOrPinRequest
    {
        [JsonPropertyName("list_id")]
        public string? ListID { get; set; }
    }
}