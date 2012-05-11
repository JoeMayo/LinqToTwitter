using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Container for mention entities
    /// </summary>
    public class Entities
    {
        public Entities() { }
        public Entities(JsonData entityJson)
        {
            if (entityJson == null) return;

            var hashTagEntities = entityJson.GetValue<JsonData>("hashtags");
            var mediaEntities = entityJson.GetValue<JsonData>("media");
            var urlEntities = entityJson.GetValue<JsonData>("urls");
            var userEntities = entityJson.GetValue<JsonData>("user_mentions");
            HashTagMentions =
                hashTagEntities == null
                    ? new List<HashTagMention>()
                    : (from JsonData hash in hashTagEntities
                       let indices = hash.GetValue<JsonData>("indices")
                       select new HashTagMention
                       {
                           Tag = hash.GetValue<string>("text"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                       .ToList();
            MediaMentions =
                mediaEntities == null
                    ? new List<MediaMention>()
                    : (from JsonData media in mediaEntities
                       let indices = media.GetValue<JsonData>("indices")
                       let sizes = media.GetValue<JsonData>("sizes")
                       select new MediaMention
                       {
                           DisplayUrl = media.GetValue<string>("display_url"),
                           ExpandedUrl = media.GetValue<string>("expanded_url"),
                           ID = media.GetValue<ulong>("id"),
                           MediaUrl = media.GetValue<string>("media_url"),
                           MediaUrlHttps = media.GetValue<string>("media_url_https"),
                           Sizes =
                               (from key in (sizes as IDictionary<string, JsonData>).Keys as List<string>
                                let sizesKey = sizes.GetValue<JsonData>(key)
                                select new PhotoSize
                                {
                                    Type = key,
                                    Width = sizesKey.GetValue<int>("w"),
                                    Height = sizesKey.GetValue<int>("h"),
                                    Resize = sizesKey.GetValue<string>("resize")
                                })
                               .ToList(),
                           Type = media.GetValue<string>("type"),
                           Url = media.GetValue<string>("url"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                       .ToList();
            UrlMentions =
                urlEntities == null
                    ? new List<UrlMention>()
                    : (from JsonData url in urlEntities
                       let indices = url.GetValue<JsonData>("indices")
                       select new UrlMention
                       {
                           Url = url.GetValue<string>("url"),
                           DisplayUrl = url.GetValue<string>("display_url"),
                           ExpandedUrl = url.GetValue<string>("expanded_url"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                      .ToList();
            UserMentions =
                userEntities == null
                    ? new List<UserMention>()
                    : (from JsonData user in userEntities
                       let indices = user.GetValue<JsonData>("indices")
                       select new UserMention
                       {
                           ScreenName = user.GetValue<string>("screen_name"),
                           Name = user.GetValue<string>("name"),
                           Id = user.GetValue<ulong>("id"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                      .ToList();
        }

        public static Entities CreateEntities(XElement element)
        {
            if (element == null || element.Descendants().Count() == 0)
            {
                return null;
            }

            //process user mentions
            var userMentions = new List<UserMention>(MentionBase.ProcessMentions<UserMention>(element, "user_mentions", "user_mention", false, UserMention.FromXElement));

            //process urls mentions
            var urlMentions = new List<UrlMention>(MentionBase.ProcessMentions<UrlMention>(element, "urls", "url", true, UrlMention.FromXElement));

            //process hashtags mentions
            var hashTagMentions = new List<HashTagMention>(MentionBase.ProcessMentions<HashTagMention>(element, "hashtags", "hashtag", false, HashTagMention.FromXElement));

            //process media mentions
            var mediaMentions = new List<MediaMention>(MentionBase.ProcessMentions<MediaMention>(element, "media", "creative", false, MediaMention.FromXElement));

            return new Entities
            {
                UserMentions = userMentions,
                HashTagMentions = hashTagMentions,
                UrlMentions = urlMentions,
                MediaMentions = mediaMentions
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

        /// <summary>
        /// Media mentions in the tweet
        /// </summary>
        public List<MediaMention> MediaMentions { get; set; }
    }
}
