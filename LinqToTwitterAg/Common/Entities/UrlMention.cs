using System.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Url mention in the tweet
    /// </summary>
    /// <example>http://bit.ly/129Ad</example>
    public class UrlMention : MentionBase
    {
        /// <summary>
        /// Absolute Url in the tweet
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// t.co shortened URL
        /// </summary>
        public string DisplayUrl { get; set; }

        /// <summary>
        /// t.co expanded URL
        /// </summary>
        public string ExpandedUrl { get; set; }
    }
}
