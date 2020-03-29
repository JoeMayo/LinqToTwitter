using LitJson;

namespace LinqToTwitter
{
    public class AspectRatio
    {
        const int WidthIndex = 0;
        const int HeightIndex = 1;

        public AspectRatio() { }
        public AspectRatio(JsonData aspectRatio)
        {
            if (aspectRatio == null) return;

            Width = (int) aspectRatio[WidthIndex];
            Height = (int) aspectRatio[HeightIndex];
        }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
