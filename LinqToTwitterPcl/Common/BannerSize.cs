using System;
using System.Xml.Serialization;

namespace LinqToTwitter.Common
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class BannerSize
    {
        public string Label { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Url { get; set; }
    }
}
