using System.Collections.Generic;
using System.Linq;
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
            if (entityJson == null)
            {
                HashTagEntities = new List<HashTagEntity>();
                MediaEntities = new List<MediaEntity>();
                UrlEntities = new List<UrlEntity>();
                UserMentionEntities = new List<UserMentionEntity>();
                SymbolEntities = new List<SymbolEntity>();

                return;
            }

            JsonData hashTagEntities = entityJson.GetValue<JsonData>("hashtags");
            JsonData mediaEntities = entityJson.GetValue<JsonData>("media");
            JsonData urlEntities = entityJson.GetValue<JsonData>("urls");
            JsonData userEntities = entityJson.GetValue<JsonData>("user_mentions");
            JsonData symbolEntities = entityJson.GetValue<JsonData>("symbols");
            HashTagEntities =
                hashTagEntities == null
                    ? new List<HashTagEntity>()
                    : (from JsonData hash in hashTagEntities
                       let indices = hash.GetValue<JsonData>("indices")
                       select new HashTagEntity
                       {
                           Tag = hash.GetValue<string>("text"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                       .ToList();
            MediaEntities =
                mediaEntities == null
                    ? new List<MediaEntity>()
                    : (from JsonData media in mediaEntities
                       let indices = media.GetValue<JsonData>("indices")
                       let sizes = media.GetValue<JsonData>("sizes")
                       select new MediaEntity
                       {
                           DisplayUrl = media.GetValue<string>("display_url"),
                           ExpandedUrl = media.GetValue<string>("expanded_url"),
                           ID = media.GetValue<ulong>("id"),
                           Indices = new List<int> { (int)indices[0], (int)indices[1] },
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
                           End = indices.Count > 1 ? (int)indices[1] : 0,
                           VideoInfo = new VideoInfo(media.GetValue<JsonData>("video_info")),
                       })
                       .ToList();
            UrlEntities =
                urlEntities == null
                    ? new List<UrlEntity>()
                    : (from JsonData url in urlEntities
                       let indices = url.GetValue<JsonData>("indices")
                       select new UrlEntity
                       {
                           Url = url.GetValue<string>("url"),
                           DisplayUrl = url.GetValue<string>("display_url"),
                           ExpandedUrl = url.GetValue<string>("expanded_url"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                      .ToList();
            UserMentionEntities =
                userEntities == null
                    ? new List<UserMentionEntity>()
                    : (from JsonData user in userEntities
                       let indices = user.GetValue<JsonData>("indices")
                       select new UserMentionEntity
                       {
                           ScreenName = user.GetValue<string>("screen_name"),
                           Name = user.GetValue<string>("name"),
                           Id = user.GetValue<ulong>("id"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                      .ToList();
            SymbolEntities =
                symbolEntities == null
                    ? new List<SymbolEntity>()
                    : (from JsonData user in symbolEntities
                       let indices = user.GetValue<JsonData>("indices")
                       select new SymbolEntity
                       {
                           Text = user.GetValue<string>("text"),
                           Start = indices.Count > 0 ? (int)indices[0] : 0,
                           End = indices.Count > 1 ? (int)indices[1] : 0
                       })
                      .ToList();
        }

        /// <summary>
        /// Mentions of the user in the tweet
        /// </summary>
        public List<UserMentionEntity> UserMentionEntities { get; set; }

        /// <summary>
        /// Url entities in the tweet
        /// </summary>
        public List<UrlEntity> UrlEntities { get; set; }

        /// <summary>
        /// Hash tag entities in the tweet
        /// </summary>
        public List<HashTagEntity> HashTagEntities { get; set; }

        /// <summary>
        /// Media entities in the tweet
        /// </summary>
        public List<MediaEntity> MediaEntities { get; set; }

        /// <summary>
        /// Symbol entities in the tweet
        /// </summary>
        public List<SymbolEntity> SymbolEntities { get; set; }
    }
}
