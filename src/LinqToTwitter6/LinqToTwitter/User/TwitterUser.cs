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
        /// Profile description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Profile location
        /// </summary>
        [JsonPropertyName("location")]
        public string? Location { get; set; }

        /// <summary>
        /// User's Twitter ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Profile URL
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Date the user signed up for Twitter
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Is a Twitter verified account?
        /// </summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        /// <summary>
        /// URL to user's profile image
        /// </summary>
        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }

        /// <summary>
        /// Entities associated with this user's profile
        /// </summary>
        [JsonPropertyName("entities")]
        public TwitterUserEntity? Entities { get; set; }

        /// <summary>
        /// Twitter assigned screen name
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        /// <summary>
        /// Metrics for this user
        /// </summary>
        [JsonPropertyName("public_metrics")]
        public TwitterUserPublicMetrics? PublicMetrics { get; set; }

        /// <summary>
        /// User's entered name in profile
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Is user account private (access by user's approval only)
        /// </summary>
        [JsonPropertyName("protected")]
        public bool Protected { get; set; }

        /// <summary>
        /// ID of tweet user has pinned at the top of their timeline
        /// </summary>
        [JsonPropertyName("pinned_tweet_id")]
        public string? PinnedTweetId { get; set; }
    }
}
