using System;
using System.Collections.Generic;
using System.Net;

namespace LinqToTwitter
{
    public interface ITwitterAuthorizer
    {
        /// <summary>
        /// Performs Twitter Authorization
        /// </summary>
        void Authorize();

        /// <summary>
        /// Prepares an authorized HTTP GET request.
        /// </summary>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        WebRequest Get(Request request);

        /// <summary>
        /// Prepares an authorized HTTP POST request.
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Parameters to be posted</param>
        /// <returns>
        /// The HTTP request.
        /// </returns>
        HttpWebRequest PostRequest(Request request, IDictionary<string, string> postData);

        // Was HttpWebResponse for calls with args
        /// <summary>
        /// Prepares and sends an authorized HTTP POST request.
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Parameters to be posted</param>
        /// <returns>
        /// The HTTP reponce.
        /// </returns>
        /// <exception cref="WebException">Thrown if the server returns an error.</exception>
        HttpWebResponse Post(Request request, IDictionary<string, string> postData);

        /// <summary>
        /// Async OAuth Post
        /// </summary>
        /// <param name="request">The request with the endpoint URL and the parameters to 
        /// include in the POST entity.  Must not be null.</param>
        /// <param name="postData">Parameters to be posted</param>
        /// <returns>HttpWebRequest for post</returns>
        HttpWebRequest PostAsync(Request request, IDictionary<string, string> postData);

        /// <summary>
        /// Gets a value indicating whether this instance is ready to send authorized GET and POST requests.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </value>
        bool IsAuthorized { get; }

        /// <summary>
        /// Gets the UserID that Twitter has assigned to the logged in user.
        /// </summary>
        /// <value>An integer number, represented as a string.</value>
        string UserId { get; }

        /// <summary>
        /// Gets the screenname of the user logged into Twitter.
        /// </summary>
        string ScreenName { get; }

        /// <summary>
        /// Gets or sets the read write timeout.
        /// </summary>
        /// <value>The read write timeout.</value>
        TimeSpan ReadWriteTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        string UserAgent { get; set; }

        /// <summary>
        /// Indicates if you want to use enable compressed responses (GZip/deflate)
        /// </summary>
        bool UseCompression { get; set; }

#if !SILVERLIGHT && !NETFX_CORE
        /// <summary>
        /// Proxy for authorization requests.
        /// </summary>
        WebProxy Proxy { get; set; }
#endif
    }
}
