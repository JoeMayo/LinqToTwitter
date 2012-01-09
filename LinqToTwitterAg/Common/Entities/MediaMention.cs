using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    public class MediaMention : MentionBase
    {
        /// <summary>
        /// Create MediaMention out of the XElement
        /// </summary>
        /// <param name="element">the entry node</param>
        /// <returns>MediaMention</returns>
        public static MediaMention FromXElement(XElement element)
        {
            var mention = new MediaMention 
            {
                ID = element.GetULong("id"),
                IDStr = element.GetString("id_str"),
                MediaUrl = element.GetString("media_url"),
                MediaUrlHttps = element.GetString("media_url_https"),
                Url = element.GetString("url"),
                DisplayUrl = element.GetString("display_url"),
                ExpandedUrl = element.GetString("expanded_url"),
                Sizes =
                    (from photo in element.Element("sizes").Elements()
                     select new PhotoSize
                     {
                         Type = photo.Name.ToString(),
                         Width = photo.GetInt("w"),
                         Height = photo.GetInt("h"),
                         Resize = photo.GetString("resize")
                     })
                    .ToList(),
                Type = element.GetString("type"),
                Indices = null
            };

            mention.SetStartEndValues(element);
            return mention;
        }

        /// <summary>
        /// ID of the media
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// String representation of media ID
        /// </summary>
        public string IDStr { get; set; }

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
