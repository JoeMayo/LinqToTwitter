using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Configuration
    {
        /// <summary>
        /// Length of a t.co short url
        /// </summary>
        public int ShortUrlLength { get; set; }

        /// <summary>
        /// Lenght of an https t.co short url
        /// </summary>
        public int ShortUrlLengthHttps { get; set; }

        /// <summary>
        /// Twitter slugs that are not usernames
        /// </summary>
        public List<string> NonUserNamePaths { get; set; }

        /// <summary>
        /// Max photo size
        /// </summary>
        public int PhotoSizeLimit { get; set; }

        /// <summary>
        /// Max number of items that can be uploaded at one time
        /// </summary>
        public int MaxMediaPerUpload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CharactersReservedPerMedia { get; set; }

        /// <summary>
        /// Sizing allowances/behaviors for each type of photo
        /// </summary>
        public List<PhotoSize> PhotoSizes { get; set; }
    }
}
