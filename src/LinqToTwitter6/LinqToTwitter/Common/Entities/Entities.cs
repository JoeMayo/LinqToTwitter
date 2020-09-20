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

            if (entityJson.IsNull())
            {
                HashTagEntities = new List<HashTagEntity>();
                MediaEntities = new List<MediaEntity>();
                UrlEntities = new List<UrlEntity>();
                UserMentionEntities = new List<UserMentionEntity>();
                SymbolEntities = new List<SymbolEntity>();

                return;
            }

            entityJson.TryGetProperty("hashtags", out JsonElement hashTagEntities);
            entityJson.TryGetProperty("media", out JsonElement mediaEntities);
            entityJson.TryGetProperty("urls", out JsonElement urlEntities);
            entityJson.TryGetProperty("user_mentions", out JsonElement userMentionEntities);
            entityJson.TryGetProperty("symbols", out JsonElement symbolEntities);

            if (hashTagEntities.IsNull())
            {
                HashTagEntities = new List<HashTagEntity>();
            }
            else
            {
                var entityAccumulator = new List<HashTagEntity>();

                foreach (var hash in hashTagEntities.EnumerateArray())
                {
                    hash.TryGetProperty("indices", out JsonElement indicesValue);
                    JsonElement[] indices = indicesValue.EnumerateArray().ToArray();

                    entityAccumulator.Add(
                        new HashTagEntity
                        {
                            Text = hash.GetString("text"),
                            Start = indices.Length > 0 ? indices[0].GetInt32() : 0,
                            End = indices.Length > 1 ? indices[1].GetInt32() : 0
                        });
                }

                HashTagEntities = entityAccumulator;
            }

            if (mediaEntities.IsNull())
            {
                MediaEntities = new List<MediaEntity>();
            }
            else
            {
                var entityAccumulator = new List<MediaEntity>();

                foreach (var media in mediaEntities.EnumerateArray())
                {
                    media.TryGetProperty("video_info", out JsonElement videoInfo);
                    media.TryGetProperty("sizes", out JsonElement sizes);
                    media.TryGetProperty("indices", out JsonElement indicesValue);
                    JsonElement[] indices = indicesValue.EnumerateArray().ToArray();

                    var sizesAccumulator = new List<PhotoSize>();

                    foreach (var photoSize in sizes.EnumerateObject())
                    {
                        sizes.TryGetProperty(photoSize.Name, out JsonElement sizesKey);
                        sizesAccumulator.Add(
                            new PhotoSize
                            {
                                Type = photoSize.Name,
                                Width = sizesKey.GetInt("w"),
                                Height = sizesKey.GetInt("h"),
                                Resize = sizesKey.GetString("resize")
                            });
                    }

                    entityAccumulator.Add(
                        new MediaEntity
                        {
                            DisplayUrl = media.GetString("display_url"),
                            ExpandedUrl = media.GetString("expanded_url"),
                            ID = media.GetUlong("id"),
                            AltText = media.GetString("ext_alt_text"),
                            Indices = new List<int> { indices[0].GetInt32(), indices[1].GetInt32() },
                            MediaUrl = media.GetString("media_url"),
                            MediaUrlHttps = media.GetString("media_url_https"),
                            Sizes = sizesAccumulator,
                            Type = media.GetProperty("type").GetString(),
                            Url = media.GetProperty("url").GetString(),
                            Start = indices[0].GetInt32(),
                            End = indices[1].GetInt32(),
                            VideoInfo = new VideoInfo(videoInfo),
                        });
                }

                MediaEntities = entityAccumulator;
            }

            if (urlEntities.IsNull())
            {
                UrlEntities = new List<UrlEntity>();
            }
            else
            {
                var entityAccumulator = new List<UrlEntity>();

                foreach (var url in urlEntities.EnumerateArray())
                {
                    url.TryGetProperty("indices", out JsonElement indicesValue);
                    JsonElement[] indices = indicesValue.EnumerateArray().ToArray();

                    entityAccumulator.Add(
                        new UrlEntity
                        {
                            Url = url.GetString("url"),
                            DisplayUrl = url.GetString("display_url"),
                            ExpandedUrl = url.GetString("expanded_url"),
                            Start = indices[0].GetInt32(),
                            End = indices[1].GetInt32()
                        });
                }

                UrlEntities = entityAccumulator;
            }

            if (userMentionEntities.IsNull())
            {
                UserMentionEntities = new List<UserMentionEntity>();
            }
            else
            {
                var entityAccumulator = new List<UserMentionEntity>();

                foreach (var user in userMentionEntities.EnumerateArray())
                {
                    user.TryGetProperty("indices", out JsonElement indicesValue);
                    JsonElement[] indices = indicesValue.EnumerateArray().ToArray();

                    entityAccumulator.Add(
                        new UserMentionEntity
                        {
                            ScreenName = user.GetString("screen_name"),
                            Name = user.GetString("name"),
                            Id = user.GetUlong("id"),
                            Start = indices[0].GetInt32(),
                            End = indices[1].GetInt32()
                        });
                }

                UserMentionEntities = entityAccumulator;
            }

            if (symbolEntities.IsNull())
            {
                SymbolEntities = new List<SymbolEntity>();
            }
            else
            {
                var entityAccumulator = new List<SymbolEntity>();

                foreach (var symbol in symbolEntities.EnumerateArray())
                {
                    symbol.TryGetProperty("indices", out JsonElement indicesValue);
                    JsonElement[] indices = indicesValue.EnumerateArray().ToArray();

                    entityAccumulator.Add(
                        new SymbolEntity
                        {
                            Text = symbol.GetString("text"),
                            Start = indices[0].GetInt32(),
                            End = indices[1].GetInt32()
                        });
                }

                SymbolEntities = entityAccumulator;
            }
        }

        /// <summary>
        /// Mentions of the user in the tweet
        /// </summary>
        [JsonPropertyName("user_mentions")]
        public List<UserMentionEntity>? UserMentionEntities { get; set; }

        /// <summary>
        /// Url entities in the tweet
        /// </summary>
        [JsonPropertyName("urls")]
        public List<UrlEntity>? UrlEntities { get; set; }

        /// <summary>
        /// Hash tag entities in the tweet
        /// </summary>
        [JsonPropertyName("hashtags")]
        public List<HashTagEntity>? HashTagEntities { get; set; }

        /// <summary>
        /// Media entities in the tweet
        /// </summary>
        [JsonPropertyName("media")]
        public List<MediaEntity>? MediaEntities { get; set; }

        /// <summary>
        /// Symbol entities in the tweet
        /// </summary>
        [JsonPropertyName("symbols")]
        public List<SymbolEntity>? SymbolEntities { get; set; }
    }
}
