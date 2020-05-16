#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LinqToTwitter.Common.Entities
{
    public class VideoInfo
    {
        public VideoInfo() { }
        public VideoInfo(JsonElement videoInfo)
        {
            AspectRatio = new AspectRatio(videoInfo.GetProperty("aspect_ratio"));
            Duration = videoInfo.GetProperty("duration_millis").GetInt32();

            if (videoInfo.TryGetProperty("variants", out JsonElement variants))
                Variants =
                    (from variant in variants.EnumerateArray()
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
