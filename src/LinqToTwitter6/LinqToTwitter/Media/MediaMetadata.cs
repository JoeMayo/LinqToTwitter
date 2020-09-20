namespace LinqToTwitter
{
    public class MediaMetadata
    {
        public ulong MediaID { get; set; }
        public AltText? AltText { get; set; }
    }

    public class AltText
    {
        public string? Text { get; set; }
    }
}
