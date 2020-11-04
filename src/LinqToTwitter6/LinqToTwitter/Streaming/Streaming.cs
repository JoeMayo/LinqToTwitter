using LinqToTwitter.Common;
using LinqToTwitter.Provider;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Reference to stream, details, and controls
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public record Streaming
    {
        /// <summary>
        /// Stream method
        /// </summary>
        public StreamingType Type { get; set; }

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

        /// <summary>
        /// Comma-separated list of rule ids, for filter rules queries
        /// </summary>
        public string? Ids { get; set; }

        /// <summary>
        /// Rule data returned from the query
        /// </summary>
        [JsonPropertyName("data")]
        public List<StreamingRule>? Rules { get; set; }

        /// <summary>
        /// If any errors occur, they'll show up here
        /// </summary>
        [JsonPropertyName("errors")]
        public List<TwitterError>? Errors { get; set; }

        /// <summary>
        /// Were there errors?
        /// </summary>
        public bool HasErrors { get => Errors?.Any() ?? false; }

        /// <summary>
        /// Tweet metadata returned from search
        /// </summary>
        [JsonPropertyName("meta")]
        public StreamingMeta? Meta { get; set; }

        /// <summary>
        /// Executor managing stream
        /// </summary>
        internal ITwitterExecute? TwitterExecutor { get; set; }

        /// <summary>
        /// Closes stream
        /// </summary>
        public void CloseStream()
        {
            TwitterExecutor?.CloseStream();
        }
    }
}
