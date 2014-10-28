using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Use for errors returned from HTTP GET and POST to Twitter
    /// </summary>
    public class TwitterQueryException : InvalidQueryException
    {
        /// <summary>
        /// init exception with general message - 
        /// you should probably use one of the other
        /// constructors for a more meaninful exception.
        /// </summary>
        public TwitterQueryException()
            : this("Twitter returned an error from your query.", null) { }

        /// <summary>
        /// init exception with custom message
        /// </summary>
        /// <param name="message">message to display</param>
        public TwitterQueryException(string message)
            : base (message, null) { }

        /// <summary>
        /// init exception with custom message and chain to originating exception
        /// </summary>
        /// <param name="message">custom message</param>
        /// <param name="inner">originating exception</param>
        public TwitterQueryException(string message, Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// provides the request url and error response message
        /// </summary>
        [Obsolete("TwitterHashResponse is deprecated. You can use ErrorCode and Message to see what Twitter's response is.")]
        public TwitterHashResponse Response { get; set; }

        /// <summary>
        /// the HTTP error from the original exception
        /// </summary>
        public string HttpError { get; set; }

        /// <summary>
        /// Error code assigned by Twitter
        /// </summary>
        public int ErrorCode { get; set; }
    }
}
