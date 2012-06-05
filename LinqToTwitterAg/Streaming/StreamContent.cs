namespace LinqToTwitter
{
    public class StreamContent : IStreamContent
    {
        private readonly ITwitterExecute exec;

        public StreamContent(ITwitterExecute exec, string content)
        {
            this.exec = exec;
            Content = content;
        }

        public string Content { get; set; }

        public virtual void CloseStream()
        {
            exec.CloseStream = true;
        }
    }
}
