using LinqToTwitter.Provider;
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
