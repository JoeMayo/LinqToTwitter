#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToTwitter.Common.Entities
{
    /// <summary>
    /// Container for mention entities
    /// </summary>
    public class Entities
    {
        public Entities() { }
        public Entities(JsonElement entityJson)
        {

            if (entityJson.ValueKind == JsonValueKind.Null)
            {
                HashTagEntities = new List<HashTagEntity>();
                MediaEntities = new List<MediaEntity>();
                UrlEntities = new List<UrlEntity>();
                UserMentionEntities = new List<UserMentionEntity>();
                SymbolEntities = new List<SymbolEntity>();

                return;
            }

            JsonElement hashTagEntities = entityJson.GetProperty("hashtags");
            JsonElement mediaEntities = entityJson.GetProperty("media");
            JsonElement urlEntities = entityJson.GetProperty("urls");
            JsonElement userEntities = entityJson.GetProperty("user_mentions");
            JsonElement symbolEntities = entityJson.GetProperty("symbols");
            HashTagEntities =
                hashTagEntities.ValueKind == JsonValueKind.Null
                    ? new List<HashTagEntity>()
                    : (from hash in hashTagEntities.EnumerateArray()
                       let indices = hash.GetProperty("indices").EnumerateArray().ToList()
                       select new HashTagEntity
                       {
                           Text = hash.GetProperty("text").GetString(),
                           Start = indices.Count > 0 ? indices[0].GetInt32() : 0,
                           End = indices.Count > 1 ? indices[1].GetInt32() : 0
                       })
                      .ToList();
            MediaEntities =
                mediaEntities.ValueKind == JsonValueKind.Null
                    ? new List<MediaEntity>()
                    : (from media in mediaEntities.EnumerateArray()
                       let indices = media.GetProperty("indices").EnumerateArray().ToList()
                       let sizes = media.GetProperty("sizes")
                       select new MediaEntity
                       {
                           DisplayUrl = media.GetProperty("display_url").GetString(),
                           ExpandedUrl = media.GetProperty("expanded_url").GetString(),
                           ID = media.GetProperty("id").GetUInt64(),
                           AltText = media.GetProperty("ext_alt_text").GetString(),
                           Indices = new List<int> { indices[0].GetInt32(), indices[1].GetInt32() },
                           MediaUrl = media.GetProperty("media_url").GetString(),
                           MediaUrlHttps = media.GetProperty("media_url_https").GetString(),
                           Sizes =
                               (from photoSize in sizes.EnumerateObject()
                                let sizesKey = sizes.GetProperty(photoSize.Name)
                                select new PhotoSize
                                {
                                    Type = photoSize.Name,
                                    Width = sizesKey.GetProperty("w").GetInt32(),
                                    Height = sizesKey.GetProperty("h").GetInt32(),
                                    Resize = sizesKey.GetProperty("resize").GetString()
                                })
                               .ToList(),
                           Type = media.GetProperty("type").GetString(),
                           Url = media.GetProperty("url").GetString(),
                           Start = indices.Count > 0 ? indices[0].GetInt32() : 0,
                           End = indices.Count > 1 ? indices[1].GetInt32() : 0,
                           VideoInfo = new VideoInfo(media.GetProperty("video_info")),
                       })
                       .ToList();
            UrlEntities =
                urlEntities.ValueKind == JsonValueKind.Null
                    ? new List<UrlEntity>()
                    : (from url in urlEntities.EnumerateArray()
                       let indices = url.GetProperty("indices").EnumerateArray().ToList()
                       select new UrlEntity
                       {
                           Url = url.GetProperty("url").GetString(),
                           DisplayUrl = url.GetProperty("display_url").GetString(),
                           ExpandedUrl = url.GetProperty("expanded_url").GetString(),
                           Start = indices.Count > 0 ? indices[0].GetInt32() : 0,
                           End = indices.Count > 1 ? indices[1].GetInt32() : 0
                       })
                      .ToList();
            UserMentionEntities =
                userEntities.ValueKind == JsonValueKind.Null
                    ? new List<UserMentionEntity>()
                    : (from user in userEntities.EnumerateArray().ToList()
                       let indices = user.GetProperty("indices").EnumerateArray().ToList()
                       select new UserMentionEntity
                       {
                           ScreenName = user.GetProperty("screen_name").GetString(),
                           Name = user.GetProperty("name").GetString(),
                           Id = user.GetProperty("id").GetUInt64(),
                           Start = indices.Count > 0 ? (int)indices[0].GetInt32() : 0,
                           End = indices.Count > 1 ? (int)indices[1].GetInt32() : 0
                       })
                      .ToList();
            SymbolEntities =
                symbolEntities.ValueKind == JsonValueKind.Null
                    ? new List<SymbolEntity>()
                    : (from user in symbolEntities.EnumerateArray()
                       let indices = user.GetProperty("indices").EnumerateArray().ToList()
                       select new SymbolEntity
                       {
                           Text = user.GetProperty("text").GetString(),
                           Start = indices.Count > 0 ? indices[0].GetInt32() : 0,
                           End = indices.Count > 1 ? indices[1].GetInt32() : 0
                       })
                      .ToList();
        }

        /// <summary>
        /// Mentions of the user in the tweet
        /// </summary>
        [JsonPropertyName("user_mentions")]
        public List<UserMentionEntity> UserMentionEntities { get; set; }

        /// <summary>
        /// Url entities in the tweet
        /// </summary>
        [JsonPropertyName("urls")]
        public List<UrlEntity> UrlEntities { get; set; }

        /// <summary>
        /// Hash tag entities in the tweet
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<HashTagEntity> HashTagEntities { get; set; }

        /// <summary>
        /// Media entities in the tweet
        /// </summary>
        [JsonPropertyName("media")]
        public List<MediaEntity> MediaEntities { get; set; }

        /// <summary>
        /// Symbol entities in the tweet
        /// </summary>
        [JsonPropertyName("symbols")]
        public List<SymbolEntity> SymbolEntities { get; set; }
    }
}
