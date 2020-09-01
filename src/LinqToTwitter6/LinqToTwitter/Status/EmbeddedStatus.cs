using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class EmbeddedStatus
    {
        public EmbeddedStatus() { }
        public EmbeddedStatus(JsonElement embeddedStatusJson)
        {
            Html = embeddedStatusJson.GetProperty("html").GetString();
            AuthorName = embeddedStatusJson.GetProperty("author_name").GetString();
            AuthorUrl = embeddedStatusJson.GetProperty("author_url").GetString();
            ProviderName = embeddedStatusJson.GetProperty("provider_name").GetString();
            ProviderUrl = embeddedStatusJson.GetProperty("provider_url").GetString();
            Url = embeddedStatusJson.GetProperty("url").GetString();
            Version = embeddedStatusJson.GetProperty("version").GetString();
            Type = embeddedStatusJson.GetProperty("type").GetString();
            Height = embeddedStatusJson.GetProperty("height").GetInt32();
            Width = embeddedStatusJson.GetProperty("width").GetInt32();
            string cacheAgeStr = embeddedStatusJson.GetProperty("cache_age").GetString();
            if (!string.IsNullOrWhiteSpace(cacheAgeStr)) 
                CacheAge = ulong.Parse(cacheAgeStr);
        }

        public string Html { get; set; }

        public string AuthorName { get; set; }

        public string ProviderUrl { get; set; }

        public string Url { get; set; }

        public string ProviderName { get; set; }

        public string Version { get; set; }

        public string Type { get; set; }

        public int Height { get; set; }

        public ulong CacheAge { get; set; }

        public string AuthorUrl { get; set; }

        public int Width { get; set; }
    }
}
