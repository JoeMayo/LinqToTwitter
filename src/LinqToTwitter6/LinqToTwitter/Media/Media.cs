using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Media
    {
        public const string StatusCommand = "STATUS";

        public Media() { }
        public Media(JsonElement media)
        {
            if (media.IsNull()) return;

            media.TryGetProperty("image", out JsonElement imageValue);
            Image = new MediaImage(imageValue);
            MediaID = media.GetUlong("media_id_string");
            Size = media.GetInt("size");
            media.TryGetProperty("video", out JsonElement video);
            VideoType = video.GetString("video_type");
            ExpiresAfterSeconds = media.GetInt("expires_after_secs");
            media.TryGetProperty("processing_info", out JsonElement processingInfoValue);
            ProcessingInfo = new MediaProcessingInfo(processingInfoValue);
        }

        public MediaType? Type { get; set; }

        public string? Command { get; set; }

        public ulong MediaID { get; set; }

        public MediaImage? Image { get; set; }

        public int Size { get; set; }

        public int ExpiresAfterSeconds { get; set; }

        public string? VideoType { get; set; }

        public MediaProcessingInfo? ProcessingInfo { get; set; }
    }
}
