using System;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class SearchEntry
    {
        /// <summary>
        /// Date of tweet (defaults to Since if Twitter doesn't return a value)
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Tweet entities (i.e. hash, media, url, or user)
        /// </summary>
        public Entities Entities { get; set; }

        /// <summary>
        /// ScreenName of tweet sender
        /// </summary>
        public string FromUser { get; set; }

        /// <summary>
        /// User ID of sender,
        /// </summary>
        public ulong FromUserID { get; set; }

        /// <summary>
        /// Name of tweet sender
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// Geo info
        /// </summary>
        public Geometry Geo { get; set; }

        /// <summary>
        /// Tweet ID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// Language of Tweet Text
        /// </summary>
        public string IsoLanguageCode { get; set; }

        /// <summary>
        /// Additional info, such as number of retweets and/or type of result
        /// </summary>
        public SearchMetaData MetaData { get; set; }

        /// <summary>
        /// Url where user's profile image is located
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// HTTPS Url where user's profile image is located
        /// </summary>
        public string ProfileImageUrlHttps { get; set; }

        /// <summary>
        /// Html-encoded anchor tag to web page for App used to send tweet
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Tweet text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ScreenName @Mention of tweet, if reply or null if not a reply
        /// </summary>
        public string ToUser { get; set; }

        /// <summary>
        /// ID for @Mention of tweet, if reply or null if not a reply
        /// </summary>
        public ulong ToUserID { get; set; }

        /// <summary>
        /// Name of user mentioned in tweet
        /// </summary>
        public string ToUserName { get; set; }
    }
}
