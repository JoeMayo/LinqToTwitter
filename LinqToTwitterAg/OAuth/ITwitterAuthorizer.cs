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
        /// <param name="url">The request URL.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        WebRequest Get(string url);

        /// <summary>
        /// Prepares an authorized HTTP POST request without sending a POST entity stream.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        HttpWebRequest Post(string url);

        /// <summary>
        /// Prepares and sends an authorized HTTP POST request.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <param name="args">The parameters to include in the POST entity.  Must not be null.</param>
        /// <returns>
        /// The HTTP response.
        /// </returns>
        /// <exception cref="WebException">Thrown if the server returns an error.</exception>
        HttpWebResponse Post(string url, Dictionary<string, string> args);

        /// <summary>
        /// Async OAuth Post
        /// </summary>
        /// <param name="url">Twitter Command</param>
        /// <param name="args">Command Arguments</param>
        /// <returns>HttpWebRequest for post</returns>
        HttpWebRequest PostAsync(string url, Dictionary<string, string> args);

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

        #region Non-authorization HTTP-specific details

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

        #endregion
    }
}
