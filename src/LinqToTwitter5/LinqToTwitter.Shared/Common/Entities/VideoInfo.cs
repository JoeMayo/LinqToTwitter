using LinqToTwitter.Common;
using LitJson;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public class VideoInfo
    {
        public VideoInfo() { }
        public VideoInfo(JsonData videoInfo)
        {
            AspectRatio = new AspectRatio(videoInfo.GetValue<JsonData>("aspect_ratio"));
            Duration = videoInfo.GetValue<int>("duration_millis");
            JsonData variants = videoInfo.GetValue<JsonData>("variants");

            if (variants != null && variants.Count > 0)
                Variants =
                    (from JsonData variant in videoInfo.GetValue<JsonData>("variants")
                     select new Variant(variant))
                    .ToList();
        }

        /// <summary>
        /// Width and Height
        /// </summary>
        public AspectRatio AspectRatio { get; set; }

        /// <summary>
        /// Duration in milliseconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Available encodings/data streams
        /// </summary>
        public List<Variant> Variants { get; set; }
    }
}
