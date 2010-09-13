//-----------------------------------------------------------------------
// <copyright file="AnonymousAuthorization.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LinqToTwitter {
	public class AnonymousAuthorization : AuthorizationBase, ITwitterAuthorization {
		/// <summary>
		/// Initializes a new instance of the <see cref="AnonymousAuthorization"/> class.
		/// </summary>
		public AnonymousAuthorization () {
		}

		#region ITwitterAuthorization Members

		/// <summary>
		/// Gets the UserID that Twitter has assigned to the logged in user.
		/// </summary>
		/// <value>An integer number, represented as a string.</value>
		public string UserId {
			get { return null; }
		}

		/// <summary>
		/// Gets the screenname of the user logged into Twitter.
		/// </summary>
		/// <value></value>
		public string ScreenName {
			get { return null; }
		}

		/// <summary>
		/// Gets or sets the base Service URI of the Twitter service to authenticate to.
		/// </summary>
		/// <value></value>
		public string AuthenticationTarget { get; set; }

		/// <summary>
		/// Gets a value indicating whether this authorization mechanism can immediately
		/// provide the user with access to his account without prompting (again)
		/// for his credentials.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
		/// </value>
		public bool CachedCredentialsAvailable {
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is ready to send authorized GET and POST requests.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
		/// </value>
		public bool IsAuthorized {
			get { return false; }
		}

		/// <summary>
		/// Logs the user into the web site, prompting for credentials if necessary.
		/// </summary>
		/// <exception cref="OperationCanceledException">Thrown if the user cancels the authentication/authorization.</exception>
		public void SignOn() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Where applicable, cancels session tokens (like an HTTP cookie), effectively logging the user off.
		/// </summary>
		public void SignOff() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the specified request URI.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public System.Net.HttpWebRequest Get(Uri requestUri, IDictionary<string, string> args) {
			Uri requestUriWithArgs = Utilities.AppendQueryString(requestUri, args);
			var req = (HttpWebRequest)WebRequest.Create(requestUriWithArgs);
			this.InitializeRequest(req);
			return req;
		}

		/// <summary>
		/// Posts the specified request URI.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <returns></returns>
		public System.Net.HttpWebRequest Post(Uri requestUri) {
			var req = (HttpWebRequest)WebRequest.Create(requestUri);
			this.InitializeRequest(req);
			req.Method = "POST";
			return req;
		}

		/// <summary>
		/// Posts the specified request URI.
		/// </summary>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public System.Net.HttpWebResponse Post(Uri requestUri, IDictionary<string, string> args) {
			if (requestUri == null) {
				throw new ArgumentNullException("requestUri");
			}

			if (args == null) {
				throw new ArgumentNullException("args");
			}

			string queryString = Utilities.BuildQueryString(args);
			byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);

			var req = (HttpWebRequest)WebRequest.Create(requestUri);
			this.InitializeRequest(req);
			req.Method = "POST";
			req.ServicePoint.Expect100Continue = false;
			req.ContentType = "x-www-form-urlencoded";
			req.ContentLength = queryStringBytes.Length;

			using (Stream requestStream = req.GetRequestStream()) {
				requestStream.Write(queryStringBytes, 0, queryStringBytes.Length);
			}

			return (HttpWebResponse)req.GetResponse();
		}

		#endregion
	}
}
