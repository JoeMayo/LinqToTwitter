namespace LinqToTwitter
{
    public interface IStreamContent
    {
        string Content { get; set; }

        void CloseStream();
    }
}