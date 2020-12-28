using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Represents a Twitter User.
    /// </summary>
    public record TwitterUser
    {
        /// <summary>
        /// Date the user signed up for Twitter
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Profile description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        /// <summary>
        /// Entities associated with this user's profile
        /// </summary>
        [JsonPropertyName("entities")]
        public TwitterUserEntity? Entities { get; init; }

        /// <summary>
        /// User's Twitter ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; init; }

        /// <summary>
        /// Profile location
        /// </summary>
        [JsonPropertyName("location")]
        public string? Location { get; init; }

        /// <summary>
        /// User's entered name in profile
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        /// ID of tweet user has pinned at the top of their timeline
        /// </summary>
        [JsonPropertyName("pinned_tweet_id")]
        public string? PinnedTweetId { get; init; }

        /// <summary>
        /// URL to user's profile image
        /// </summary>
        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; init; }

        /// <summary>
        /// Is user account private (access by user's approval only)
        /// </summary>
        [JsonPropertyName("protected")]
        public bool Protected { get; init; }

        /// <summary>
        /// Metrics for this user
        /// </summary>
        [JsonPropertyName("public_metrics")]
        public TwitterUserPublicMetrics? PublicMetrics { get; init; }

        /// <summary>
        /// Profile URL
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; }

        /// <summary>
        /// Twitter assigned screen name
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; init; }

        /// <summary>
        /// Is a Twitter verified account?
        /// </summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; init; }

        /// <summary>
        /// Details on information to withhold
        /// </summary>
        public object? Withheld { get; set; }
    }
}
