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

            Image = new MediaImage(media.GetProperty("image"));
            MediaID = media.GetProperty("media_id_string").GetUInt64();
            Size = media.GetProperty("size").GetInt32();
            JsonElement video = media.GetProperty("video");
            VideoType = video.GetProperty("video_type").GetString();
            ExpiresAfterSeconds = media.GetProperty("expires_after_secs").GetInt32();
            ProcessingInfo = new MediaProcessingInfo(media.GetProperty("processing_info"));
        }

        public MediaType Type { get; set; }

        public string Command { get; set; }

        public ulong MediaID { get; set; }

        public MediaImage Image { get; set; }

        public int Size { get; set; }

        public int ExpiresAfterSeconds { get; set; }

        public string VideoType { get; set; }

        public MediaProcessingInfo ProcessingInfo { get; set; }
    }
}
