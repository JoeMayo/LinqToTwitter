using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public class MediaMention : MentionBase
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
        /// Media Url extracted
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// String to display for Url
        /// </summary>
        public string DisplayUrl { get; set; }

        /// <summary>
        /// Media Url that has been fully resolved
        /// </summary>
        public string ExpandedUrl { get; set; }

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
