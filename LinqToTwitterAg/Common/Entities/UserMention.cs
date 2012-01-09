using System;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Twitter user mention in the tweet
    /// </summary>
    /// <example>@linkedin</example>
    public class UserMention : MentionBase
    {
        /// <summary>
        /// Create UserMention out of the XElement
        /// </summary>
        /// <param name="element">the entry node</param>
        /// <returns>UserMention</returns>
        public static UserMention FromXElement(XElement element)
        {
            var mention = new UserMention
            {
                Id = Int64.Parse(element.Element("id").Value),
                Name = element.Element("name").Value,
                ScreenName = element.Element("screen_name").Value
            };

            mention.SetStartEndValues(element);
            return mention;
        }

        /// <summary>
        /// Tweitter user Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Screen name of the Twitter User
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Name of the Twitter User
        /// </summary>
        public string Name { get; set; }
    }
}
