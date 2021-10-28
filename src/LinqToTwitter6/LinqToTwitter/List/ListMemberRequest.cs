using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record ListMemberRequest
    {
        [JsonPropertyName("user_id")]
        public string? UserID { get; set; }
    }
}