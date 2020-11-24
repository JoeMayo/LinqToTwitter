using LinqToTwitter.Common;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record Tweet
    {
        /// <summary>
        /// ID of this tweet
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; init; }

        /// <summary>
        /// User's tweet text
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; init; }

        [JsonPropertyName("attachments")]
        public TweetAttachments? Attachments { get; init; }

        /// <summary>
        /// Posting user's ID
        /// </summary>
        [JsonPropertyName("author_id")]
        public string? AuthorID { get; init; }

        /// <summary>
        /// Annotations for this tweet
        /// </summary>
        [JsonPropertyName("context_annotations")]
        public List<TweetContextAnnotation>? ContextAnnotations { get; init; }

        /// <summary>
        /// ID of conversation this tweet is part of (matches original tweet ID)
        /// </summary>
        [JsonPropertyName("conversation_id")]
        public string? ConversationID { get; init; }

        /// <summary>
        /// When tweeted
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; init; }

        /// <summary>
        /// Extracted parts of the tweet, like hashtags, urls, etc.
        /// </summary>
        [JsonPropertyName("entities")]
        public TweetEntities? Entities { get; init; }

        /// <summary>
        /// Tweet location (if user provided)
        /// </summary>
        [JsonPropertyName("geo")]
        public TweetGeo? Geo { get; init; }

        /// <summary>
        /// If replying, this is the ID of the user being replied to
        /// </summary>
        [JsonPropertyName("in_reply_to_user_id")]
        public string? InReplyToUserID { get; init; }

        /// <summary>
        /// BCP47 Language tag - https://tools.ietf.org/html/bcp47
        /// </summary>
        [JsonPropertyName("lang")]
        public string? Language { get; init; }

        // TODO: finish implementation - requires permissions to access
        /// <summary>
        /// Non-Public Metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("non_public_metrics")]
        public object? NonPublicMetrics { get; init; }

        /// <summary>
        /// Organic Metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("organic_metrics")]
        public object? OrganicMetrics { get; init; }

        /// <summary>
        /// Media or links might reveal sensitive information
        /// </summary>
        [JsonPropertyName("possibly_sensitive")]
        public bool? PossiblySensitive { get; init; }

        /// <summary>
        /// Promoted content metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("promoted_metrics")]
        public object? PromotedMetrics { get; init; }

        /// <summary>
        /// Public metrics
        /// </summary>
        [JsonPropertyName("public_metrics")]
        public TweetPublicMetrics? PublicMetrics { get; init; }

        /// <summary>
        /// Any other tweets that this one refers to
        /// </summary>
        [JsonPropertyName("referenced_tweets")]
        public List<TweetReference>? ReferencedTweets { get; init; }

        /// <summary>
        /// The application sending the tweet
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; init; }

        /// <summary>
        /// Information regarding a request to withhold information
        /// </summary>
        [JsonPropertyName("withheld")]
        public TwitterWithheld? Withheld { get; init; }
    }
}