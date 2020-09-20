using System.Xml.Serialization;
using System.Text.Json;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class MediaImage
    {
        public MediaImage(JsonElement image)
        {
            if (image.IsNull()) return;

            Width = image.GetInt("w");
            Height = image.GetInt("h");
            ImageType = image.GetString("image_type");
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public string? ImageType { get; set; }
    }
}
