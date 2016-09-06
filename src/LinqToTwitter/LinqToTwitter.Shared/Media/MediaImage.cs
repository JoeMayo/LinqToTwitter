using System;
using System.Linq;
using LitJson;
using LinqToTwitter.Common;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class MediaImage
    {
        public MediaImage(JsonData image)
        {
            Width = image.GetValue<int>("w");
            Height = image.GetValue<int>("h");
            ImageType = image.GetValue<string>("image_type");
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageType { get; set; }
    }
}
