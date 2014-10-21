using LitJson;
using System;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    public class Media
    {
        public Media() { }
        public Media(JsonData media)
        {
            if (media == null) return;

            Image = new MediaImage(media.GetValue<JsonData>("image"));
            MediaID = media.GetValue<string>("media_id_string").GetULong(0);
            Size = media.GetValue<int>("size");
        }

        public MediaType Type { get; set; }

        public MediaImage Image { get; set; }

        public ulong MediaID { get; set; }

        public int Size { get; set; }
    }
}
