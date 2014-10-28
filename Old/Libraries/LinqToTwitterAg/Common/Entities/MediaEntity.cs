using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
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
        public string MediaUrl { get; set; }

        /// <summary>
        /// SSL version of media URL
        /// </summary>
        public string MediaUrlHttps { get; set; }

        /// <summary>
        /// Supported media sizes
        /// </summary>
        public List<PhotoSize> Sizes { get; set; }

        /// <summary>
        /// Type of media
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Character positions of extracted media
        /// </summary>
        public List<int> Indices { get; set; }
    }
}
