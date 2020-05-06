namespace LinqToTwitter.Streaming
{
    public interface IStreamContent
    {
        string Content { get; set; }

        void CloseStream();
    }
}