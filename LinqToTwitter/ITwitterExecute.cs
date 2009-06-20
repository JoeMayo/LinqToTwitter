using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace LinqToTwitter
{
    /// <summary>
    /// Members for communicating with Twitter
    /// </summary>
    public interface ITwitterExecute
    {
        /// <summary>
        /// login name of user
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// user's password
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// list of response headers from query
        /// </summary>
        Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// Timeout (milliseconds) for writing to request 
        /// stream or reading from response stream
        /// </summary>
        int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Timeout (milliseconds) to wait for a server response
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// True if OAuth succeeds, otherwise false.
        /// </summary>
        bool AuthorizedViaOAuth { get; }

        /// <summary>
        /// Twitter OAuth Implementation
        /// </summary>
        IOAuthTwitter OAuthTwitter { get; set; }

        /// <summary>
        /// utility method to perform HTTP POST for Twitter requests with side-effects
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="parameters">parameters to post</param>
        /// <param name="requestProcessor">IRequestProcessor to handle response</param>
        /// <returns>response from server, handled by the requestProcessor</returns>
        IList ExecuteTwitter(string url, Dictionary<string, string> parameters, IRequestProcessor requestProcessor);

        /// <summary>
        /// performs HTTP POST file upload to Twitter
        /// </summary>
        /// <param name="fileName">name of file to upload</param>
        /// <param name="url">url to upload to</param>
        /// <returns>IQueryable</returns>
        IList PostTwitterFile(string filePath, Dictionary<string, string> parameters, string url, IRequestProcessor requestProcessor);

        /// <summary>
        /// makes HTTP call to Twitter API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <returns>List of objects to return</returns>
        IList QueryTwitter(string url, IRequestProcessor requestProcessor);
    }
}
