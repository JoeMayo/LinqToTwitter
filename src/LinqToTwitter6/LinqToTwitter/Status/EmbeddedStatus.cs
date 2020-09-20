using System.Xml.Serialization;
using System.Text.Json;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class EmbeddedStatus
    {
        public EmbeddedStatus() { }
        public EmbeddedStatus(JsonElement embeddedStatusJson)
        {
            Html = embeddedStatusJson.GetString("html");
            AuthorName = embeddedStatusJson.GetString("author_name");
            AuthorUrl = embeddedStatusJson.GetString("author_url");
            ProviderName = embeddedStatusJson.GetString("provider_name");
            ProviderUrl = embeddedStatusJson.GetString("provider_url");
            Url = embeddedStatusJson.GetString("url");
            Version = embeddedStatusJson.GetString("version");
            Type = embeddedStatusJson.GetString("type");
            Height = embeddedStatusJson.GetInt("height");
            Width = embeddedStatusJson.GetInt("width");
            string? cacheAgeStr = embeddedStatusJson.GetString("cache_age");
            if (!string.IsNullOrWhiteSpace(cacheAgeStr)) 
                CacheAge = ulong.Parse(cacheAgeStr);
        }

        public string? Html { get; set; }

        public string? AuthorName { get; set; }

        public string? ProviderUrl { get; set; }

        public string? Url { get; set; }

        public string? ProviderName { get; set; }

        public string? Version { get; set; }

        public string? Type { get; set; }

        public int Height { get; set; }

        public ulong CacheAge { get; set; }

        public string? AuthorUrl { get; set; }

        public int Width { get; set; }
    }
}
