//-----------------------------------------------------------------------
// <copyright file="Utilities.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace LinqToTwitter {
    /// <summary>
    /// Provides steps to authorize a Twitter client and send authorized HTTP requests.
    /// </summary>
    public interface ITwitterAuthorization
    {
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

        #endregion

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
        /// Gets or sets the base Service URI of the Twitter service to authenticate to.
        /// </summary>
        string AuthenticationTarget { get; set;  }

        /// <summary>
        /// Gets a value indicating whether this authorization mechanism can immediately
        /// provide the user with access to his account without prompting (again)
        /// for his credentials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
        /// </value>
        bool CachedCredentialsAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready to send authorized GET and POST requests.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </value>
        bool IsAuthorized { get; }

        /// <summary>
        /// Logs the user into the web site, prompting for credentials if necessary.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the authentication/authorization.</exception>
        void SignOn();

        /// <summary>
        /// Where applicable, cancels session tokens (like an HTTP cookie), effectively logging the user off.
        /// </summary>
        void SignOff();

        /// <summary>
        /// Clears the cached credentials, if any.
        /// </summary>
        void ClearCachedCredentials();

        /// <summary>
        /// Prepares an authorized HTTP GET request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The arguments to include in the query string.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        HttpWebRequest Get(Uri requestUrl, IDictionary<string, string> args);

        /// <summary>
        /// Prepares an authorized HTTP POST request without sending a POST entity stream.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        HttpWebRequest Post(Uri requestUrl);

        /// <summary>
        /// Prepares and sends an authorized HTTP POST request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The parameters to include in the POST entity.  Must not be null.</param>
        /// <returns>
        /// The HTTP response.
        /// </returns>
        /// <exception cref="WebException">Thrown if the server returns an error.</exception>
        HttpWebResponse Post(Uri requestUrl, IDictionary<string, string> args);
    }
}
