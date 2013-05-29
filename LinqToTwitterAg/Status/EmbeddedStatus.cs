using System;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class EmbeddedStatus
    {
        public EmbeddedStatus() { }
        public EmbeddedStatus(JsonData embeddedStatusJson)
        {
            Html = embeddedStatusJson.GetValue<string>("html");
            AuthorName = embeddedStatusJson.GetValue<string>("author_name");
            AuthorUrl = embeddedStatusJson.GetValue<string>("author_url");
            ProviderName = embeddedStatusJson.GetValue<string>("provider_name");
            ProviderUrl = embeddedStatusJson.GetValue<string>("provider_url");
            Url = embeddedStatusJson.GetValue<string>("url");
            Version = embeddedStatusJson.GetValue<string>("version");
            Type = embeddedStatusJson.GetValue<string>("type");
            Height = embeddedStatusJson.GetValue<int>("height");
            Width = embeddedStatusJson.GetValue<int>("width");
            CacheAge = embeddedStatusJson.GetValue<ulong>("cache_age");
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
