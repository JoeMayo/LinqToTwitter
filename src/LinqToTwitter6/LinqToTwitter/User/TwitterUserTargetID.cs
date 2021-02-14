using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TwitterUserTargetID
    {
        [JsonPropertyName("target_user_id")]
        public string? TargetUserID { get; init; }
    }
}