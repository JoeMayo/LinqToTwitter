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
            if (videoInfo.IsNull())
                return;

            videoInfo.TryGetProperty("aspect_ratio", out JsonElement aspectRatio);
            AspectRatio = new AspectRatio(aspectRatio);
            Duration = videoInfo.GetInt("duration_millis");

            if (videoInfo.TryGetProperty("variants", out JsonElement variants))
                Variants =
                    (from variant in variants.EnumerateArray()
                     select new Variant(variant))
                    .ToList();
        }

        /// <summary>
        /// Width and Height
        /// </summary>
        public AspectRatio? AspectRatio { get; set; }

        /// <summary>
        /// Duration in milliseconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Available encodings/data streams
        /// </summary>
        public List<Variant>? Variants { get; set; }
    }
}
