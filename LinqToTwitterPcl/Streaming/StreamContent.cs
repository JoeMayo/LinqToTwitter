using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Response from Twitter stream
    /// </summary>
    public class StreamContent : IStreamContent
    {
        private readonly ITwitterExecute exec;

        public StreamContent(ITwitterExecute exec, string content)
        {
            this.exec = exec;
            Content = content;
        }

        /// <summary>
        /// Stream object, which is a Twitter message of various
        /// formats or empty string for keep-alive message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Closes Twitter stream.
        /// </summary>
        public virtual void CloseStream()
        {
            exec.CloseStream();
        }
    }
}
