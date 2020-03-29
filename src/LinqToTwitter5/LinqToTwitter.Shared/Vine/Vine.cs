using LinqToTwitter.Common;
using LitJson;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Vine
    {
        public Vine() { }
        public Vine(JsonData vine)
        {
            if (vine == null) return;

            Version = vine.GetValue<double>("version");
            TypeResponse = vine.GetValue<string>("type");
            CacheAge = vine.GetValue<long>("cache_age");
            ProviderName = vine.GetValue<string>("provider_name");
            ProviderUrl = vine.GetValue<string>("provider_url");
            AuthorName = vine.GetValue<string>("author_name");
            AuthorUrl = vine.GetValue<string>("author_url");
            Title = vine.GetValue<string>("title");
            ThumbnailUrl = vine.GetValue<string>("thumbnail_url");
            ThumbnailWidth = vine.GetValue<int>("thumbnail_width");
            ThumbnailHeight = vine.GetValue<int>("thumbnail_height");
            Html = vine.GetValue<string>("html");
            Width = vine.GetValue<int>("width");
            Height = vine.GetValue<int>("height");
        }

        //
        // input filters
        //

        /// <summary>
        /// Type of vine query to perform.
        /// </summary>
        public VineType Type { get; set; }

        /// <summary>
        /// ID of vine to query
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Url of vine to query.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Maximum width of script.
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Maximum height of script.
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// Don't include script.
        /// </summary>
        public bool OmitScript { get; set; }

        //
        // response output
        //

        /// <summary>
        /// Vine version.
        /// </summary>
        public double Version { get; set; }

        /// <summary>
        /// Type of response. e.g. video
        /// </summary>
        public string TypeResponse { get; set; }

        /// <summary>
        /// Age of cache
        /// </summary>
        public long CacheAge { get; set; }

        /// <summary>
        /// Video provider
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// URL of video provider
        /// </summary>
        public string ProviderUrl { get; set; }

        /// <summary>
        /// Person who published video
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// URL for person who published video
        /// </summary>
        public string AuthorUrl { get; set; }

        /// <summary>
        /// Name of video
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Still thumbnail image from video
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Width of thumbnail image
        /// </summary>
        public int ThumbnailWidth { get; set; }

        /// <summary>
        /// Height of thumbnail image
        /// </summary>
        public int ThumbnailHeight { get; set; }

        /// <summary>
        /// Html/script for embedded video
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// Video width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Video height
        /// </summary>
        public int Height { get; set; }
    }
}
