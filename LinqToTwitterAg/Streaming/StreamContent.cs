namespace LinqToTwitter
{
    public class StreamContent
    {
        private readonly ITwitterExecute exec;

        public StreamContent(ITwitterExecute exec, string content)
        {
            this.exec = exec;
            Content = content;
        }

        public string Content { get; set; }

        public void CloseStream()
        {
            exec.CloseStream = true;
        }
    }
}
