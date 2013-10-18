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

//#if !NETFX_CORE
//        /// <summary>
//        /// performs HTTP POST file upload to Twitter
//        /// </summary>
//        /// <param name="filePath">full path of file to upload</param>
//        /// <param name="parameters">query string parameters</param>
//        /// <param name="url">url to upload to</param>
//        /// <returns>XML Results from Twitter</returns>
//        string PostTwitterFile<T>(string url, IDictionary<string, string> postData, string filePath, IRequestProcessor<T> reqProc);
//#endif

//        /// <summary>
//        /// performs HTTP POST image byte array upload to Twitter
//        /// </summary>
//        /// <param name="image">byte array containing image to upload</param>
//        /// <param name="url">url to upload to</param>
//        /// <param name="fileName">name to pass to Twitter for the file</param>
//        /// <param name="imageType">type of image: must be one of jpg, gif, or png</param>
//        /// <returns>XML results From Twitter</returns>
//        string PostTwitterImage<T>(string url, IDictionary<string, string> postData, byte[] image, string fileName, string imageType, IRequestProcessor<T> reqProc);

//        /// <summary>
//        /// performs HTTP POST media byte array upload to Twitter
//        /// </summary>
//        /// <param name="url">url to upload to</param>
//        /// <param name="postData">request parameters</param>
//        /// <param name="mediaItems">list of Media each media item to upload</param>
//        /// <param name="reqProc">request processor for handling results</param>
//        /// <returns>XML results From Twitter</returns>
//        string PostMedia<T>(string url, IDictionary<string, string> postData, List<Media> mediaItems, IRequestProcessor<T> reqProc);

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>XML Results from Twitter</returns>
        Task<string> QueryTwitterAsync<T>(Request req, IRequestProcessor<T> reqProc);

        /// <summary>
        /// Query for Twitter Streaming APIs
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>Raw results from Twitter</returns>
        string QueryTwitterStream(Request req);

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
