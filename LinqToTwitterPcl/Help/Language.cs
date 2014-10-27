using System.Xml.Serialization;
namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Language
    {
        /// <summary>
        /// Long description of language name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Two character major language code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Twitter status
        /// </summary>
        public string Status { get; set; }
    }
}
