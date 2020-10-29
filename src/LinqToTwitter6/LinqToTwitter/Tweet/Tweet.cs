using LinqToTwitter.Common;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record Tweet
    {
        /*
         * Query input fields
         */

        /// <summary>
        /// type of search, included for compatibility
        /// with other APIs
        /// </summary>
        public TweetType Type { get; set; }

        /// <summary>
        /// Required - Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the media object
        /// </summary>
        public string? MediaFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the place object
        /// </summary>
        public string? PlaceFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the poll object
        /// </summary>
        public string? PollFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object
        /// </summary>
        public string? TweetFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object
        /// </summary>
        public string? UserFields { get; set; }

        /*
         * Filled in on Twitter Response
         */
        /// <summary>
        /// ID of this tweet
        /// </summary>
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// User's tweet text
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("attachments")]
        public TweetAttachments? Attachments { get; set; }

        /// <summary>
        /// Posting user's ID
        /// </summary>
        [JsonPropertyName("author_id")]
        public string? AuthorID { get; set; }

        /// <summary>
        /// Annotations for this tweet
        /// </summary>
        [JsonPropertyName("context_annotations")]
        public List<TweetContextAnnotation>? ContextAnnotations { get; set; }

        /// <summary>
        /// ID of conversation this tweet is part of (matches original tweet ID)
        /// </summary>
        [JsonPropertyName("conversation_id")]
        public string? ConversationID { get; set; }

        /// <summary>
        /// When tweeted
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Extracted parts of the tweet, like hashtags, urls, etc.
        /// </summary>
        [JsonPropertyName("entities")]
        public TweetEntities? Entities { get; set; }

        /// <summary>
        /// Tweet location (if user provided)
        /// </summary>
        [JsonPropertyName("geo")]
        public TweetGeo? Geo { get; set; }

        /// <summary>
        /// If replying, this is the ID of the user being replied to
        /// </summary>
        [JsonPropertyName("in_reply_to_user_id")]
        public string? InReplyToUserID { get; set; }

        /// <summary>
        /// BCP47 Language tag - https://tools.ietf.org/html/bcp47
        /// </summary>
        [JsonPropertyName("lang")]
        public string? Language { get; set; }

        // TODO: finish implementation - requires permissions to access
        /// <summary>
        /// Non-Public Metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("non_public_metrics")]
        public object? NonPublicMetrics { get; set; }

        /// <summary>
        /// Organic Metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("organic_metrics")]
        public object? OrganicMetrics { get; set; }

        /// <summary>
        /// Media or links might reveal sensitive information
        /// </summary>
        [JsonPropertyName("possiby_sensitive")]
        public bool? PossiblySensitive { get; set; }

        /// <summary>
        /// Promoted content metrics
        /// </summary>
        // TODO: finish implementation - requires permissions to access
        [JsonPropertyName("promoted_metrics")]
        public object? PromotedMetrics { get; set; }

        /// <summary>
        /// Public metrics
        /// </summary>
        [JsonPropertyName("public_metrics")]
        public TweetPublicMetrics? PublicMetrics { get; set; }

        /// <summary>
        /// Any other tweets that this one refers to
        /// </summary>
        [JsonPropertyName("referenced_tweets")]
        public List<TweetReference>? ReferencedTweets { get; set; }

        /// <summary>
        /// The application sending the tweet
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// Information regarding a request to withhold information
        /// </summary>
        [JsonPropertyName("withheld")]
        public TwitterWithheld? Withheld { get; set; }
    }
}