using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class MediaImage
    {
        public MediaImage(JsonElement image)
        {
            Width = image.GetProperty("w").GetInt32();
            Height = image.GetProperty("h").GetInt32();
            ImageType = image.GetProperty("image_type").GetString();
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageType { get; set; }
    }
}
