using System.Collections.Generic;

namespace LinqToTwitter.Common.Entities
{
    public class MediaEntity : UrlEntity
    {
        /// <summary>
        /// ID of the media
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// URL where media is located
        /// </summary>
        public string? MediaUrl { get; set; }

        /// <summary>
        /// Populated with media alt text, if available.
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// SSL version of media URL
        /// </summary>
        public string? MediaUrlHttps { get; set; }

        /// <summary>
        /// Supported media sizes
        /// </summary>
        public List<PhotoSize>? Sizes { get; set; }

        /// <summary>
        /// Type of media
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Character positions of extracted media
        /// </summary>
        public new List<int>? Indices { get; set; }

        public VideoInfo? VideoInfo { get; set; }
    }
}
