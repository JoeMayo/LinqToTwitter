using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Url mention in the tweet
    /// </summary>
    /// <example>http://bit.ly/129Ad</example>
    public class UrlMention : MentionBase
    {
        /// <summary>
        /// Create UrlMention out of the XElement
        /// </summary>
        /// <param name="element">the entry node</param>
        /// <returns>UrlMention</returns>
        public static UrlMention FromXElement(XElement element)
        {
            var mention = new UrlMention 
            { 
                Url = element.Element("url") == null ? "" : element.Element("url").Value,
                DisplayUrl = element.Element("display_url") == null ? "" : element.Element("display_url").Value,
                ExpandedUrl = element.Element("expanded_url") == null ? "" : element.Element("expanded_url").Value
            };

            mention.SetStartEndValues(element);
            return mention;
        }

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
