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

            Status = TwitterErrorStatus.Success;
        }

        /// <summary>
        /// Stream object, which is a Twitter message of various
        /// formats or empty string for keep-alive message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// You can check this value in the callback to
        /// see if the request succeeded or failed
        /// </summary>
        public TwitterErrorStatus Status { get; set; }

        /// <summary>
        /// Will contain Exception if there was an error
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Closes Twitter stream.
        /// </summary>
        public virtual void CloseStream()
        {
            exec.CloseStream();
        }
    }
}
