using System.Linq;
using System.Text.Json;

namespace LinqToTwitter.Common.Entities
{
    public class AspectRatio
    {
        const int WidthIndex = 0;
        const int HeightIndex = 1;

        public AspectRatio() { }
        public AspectRatio(JsonElement aspectRatio)
        {
            var aspectRatioArray = aspectRatio.EnumerateArray().ToArray();
            Width = aspectRatioArray[WidthIndex].GetInt32();
            Height = aspectRatioArray[HeightIndex].GetInt32();
        }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
