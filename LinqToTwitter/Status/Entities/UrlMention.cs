using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Url mention in the tweet
    /// </summary>
    /// <example>http://bit.ly/129Ad</example>
    [Serializable]
    public class UrlMention : MentionBase
    {
        /// <summary>
        /// Absolute Url in the tweet
        /// </summary>
        public string Url { get; set; }
    }
}
