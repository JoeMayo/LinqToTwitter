using LitJson;
using System;
using LinqToTwitter.Common;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Media
    {
        public const string StatusCommand = "STATUS";

        public Media() { }
        public Media(JsonData media)
        {
            if (media == null) return;

            Image = new MediaImage(media.GetValue<JsonData>("image"));
            MediaID = media.GetValue<string>("media_id_string").GetULong(0);
            Size = media.GetValue<int>("size");
            JsonData video = media.GetValue<JsonData>("video");
            VideoType = video.GetValue<string>("video_type");
            ExpiresAfterSeconds = media.GetValue<int>("expires_after_secs");
            ProcessingInfo = new MediaProcessingInfo(media.GetValue<JsonData>("processing_info"));
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
