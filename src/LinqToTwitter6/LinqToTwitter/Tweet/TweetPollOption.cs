using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// An option in a Twitter poll
    /// </summary>
    public record TweetPollOption
    {
        /// <summary>
        /// Ordinal of the option
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; init; }

        /// <summary>
        /// Text of option
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; init; }

        /// <summary>
        /// How many people voted for this option
        /// </summary>
        [JsonPropertyName("votes")]
        public int Votes { get; init; }
    }
}