using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class PhotoSize
    {
        /// <summary>
        /// Type of photo (i.e. Large, Small, etc.)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Photo Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Photo Height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Resize Behavior (i.e. crop, fit, ...)
        /// </summary>
        public string Resize { get; set; }
    }
}
