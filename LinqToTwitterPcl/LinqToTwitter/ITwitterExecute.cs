using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    /// <summary>
    /// Members for communicating with Twitter
    /// </summary>
    public interface ITwitterExecute
    {
        /// <summary>
        /// Gets or sets the object that can send authorized requests to Twitter.
        /// </summary>
        IAuthorizer Authorizer { get; set; }

        /// <summary>
        /// Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        /// This is very useful for debugging
        /// </remarks>
        string LastUrl { get; }

        /// <summary>
        /// list of response headers from query
        /// </summary>
        IDictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        string UserAgent { get; set; }

        ///// <summary>
        ///// Timeout (milliseconds) for writing to request 
        ///// stream or reading from response stream
        ///// </summary>
        //int ReadWriteTimeout { get; set; }

        ///// <summary>
        ///// Timeout (milliseconds) to wait for a server response
        ///// </summary>
        //int Timeout { get; set; }

        /// <summary>
        /// performs HTTP POST to Twitter
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="postData">parameters to post</param>
        /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
        /// <returns>Json Response from Twitter - empty string if async</returns>
        Task<string> PostToTwitterAsync<T>(string url, IDictionary<string, string> postData);

        /// <summary>
        /// performs HTTP POST media byte array upload to Twitter
        /// </summary>
        /// <param name="url">url to upload to</param>
        /// <param name="postData">request parameters</param>
        /// <param name="image">Image data in a byte[]</param>
        /// <param name="name">Name of parameter to pass to Twitter.</param>
        /// <param name="fileName">name to pass to Twitter for the file</param>
        /// <param name="contentType">Type of image: must be one of jpg, gif, or png</param>
        /// <returns>JSON results From Twitter</returns>
        Task<string> PostMediaAsync(string url, IDictionary<string, string> postData, byte[] image, string name, string fileName, string contentType);

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>JSON Results from Twitter</returns>
        Task<string> QueryTwitterAsync<T>(Request req, IRequestProcessor<T> reqProc);

        /// <summary>
        /// Query for Twitter Streaming APIs
        /// </summary>
        /// <param name="req">Request URL and parameters.</param>
        /// <returns>Placeholder - real data flows from stream into callback you define.</returns>
        Task<string> QueryTwitterStreamAsync(Request req);

        /// <summary>
        /// Used to notify callers of changes in image upload progress
        /// </summary>
        event EventHandler<TwitterProgressEventArgs> UploadProgressChanged;

        /// <summary>
        /// Allows users to process content returned from stream
        /// </summary>
        Action<StreamContent> StreamingCallback { get; set; }

        /// <summary>
        /// Set to true to close stream, false means stream is still open
        /// </summary>
        bool CloseStream { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        string StreamingUserName { get; set; }

        /// <summary>
        /// Only for streaming credentials, use OAuth for non-streaming APIs
        /// </summary>
        string StreamingPassword { get; set; }

        /// <summary>
        /// Executed for async calls
        /// </summary>
        Delegate AsyncCallback { get; set; }
    }
}
