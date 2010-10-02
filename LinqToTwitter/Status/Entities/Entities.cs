using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Container for mention entities
    /// </summary>
    [Serializable]
    public class Entities
    {
        public Entities CreateEntities(XElement element)
        {
            if (element == null || element.Descendants().Count() == 0)
            {
                return null;
            }

            //process user mentions
            var userMentions = new List<UserMention>();
            if(element.Element("user_mentions") != null && element.Element("user_mentions").HasElements)
            {
                userMentions.AddRange(element.Element("user_mentions").Descendants("user_mention").Select(GetUserMention));
            }

            //process urls mentions
            var urlMentions = new List<UrlMention>();
            if (element.Element("urls") != null && element.Element("urls").HasElements)
            {
                urlMentions.AddRange(element.Element("urls").Descendants("url").Where(x => x.Attribute("start")!=null).Select(GetUrlMention));
            }

            //process hashtags mentions
            var hashTagMentions = new List<HashTagMention>();
            if (element.Element("hashtags") != null && element.Element("hashtags").HasElements)
            {
                hashTagMentions.AddRange(element.Element("hashtags").Descendants("hashtag").Select(GetHashTagMention));
            }

            return new Entities
                               {
                                   UserMentions = userMentions,
                                   HashTagMentions = hashTagMentions,
                                   UrlMentions = urlMentions
                               };
        }

        /// <summary>
        /// Mentions of the user
        /// </summary>
        public List<UserMention> UserMentions { get; set; }

        /// <summary>
        /// Url mentions in the tweet
        /// </summary>
        public List<UrlMention> UrlMentions { get; set; }

        /// <summary>
        /// Hash tags mentions in the tweet
        /// </summary>
        public List<HashTagMention> HashTagMentions { get; set; }

        #region Helpers

        /// <summary>
        /// Create object out of the XElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private UserMention GetUserMention(XElement element)
        {
            var mention = new UserMention
                              {
                                  Id = Int64.Parse(element.Element("id").Value),
                                  Name = element.Element("name").Value,
                                  ScreenName = element.Element("screen_name").Value
                              };

            SetValues(mention, element);
            return mention;
        }

        private HashTagMention GetHashTagMention(XElement element)
        {
            var mention = new HashTagMention {Tag = element.Value};

            SetValues(mention, element);
            return mention;
        }

        private UrlMention GetUrlMention(XElement element)
        {
            var mention = new UrlMention { Url = element.Element("url").Value };

            SetValues(mention, element);
            return mention;
        }

        private void SetValues(MentionBase mention, XElement element)
        {
            mention.Start = Int32.Parse(element.Attribute("start").Value);
            mention.End = Int32.Parse(element.Attribute("end").Value);
        }

        #endregion
    }
}
