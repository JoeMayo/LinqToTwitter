//-----------------------------------------------------------------------
// <copyright file="AuthorizationBase.cs">
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

namespace LinqToTwitter {
	using System.Net;

	public abstract class AuthorizationBase {
		#region ITwitterAuthorization Members

		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>The user agent.</value>
		public string UserAgent { get; set; }

		/// <summary>
		/// Indicates if you want to use enable compressed responses (GZip/deflate)
		/// </summary>
		public bool UseCompression { get; set; }

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Gets or sets the read write timeout.
		/// </summary>
		/// <value>The read write timeout.</value>
		public TimeSpan ReadWriteTimeout { get; set; }

		#endregion

		/// <summary>
		/// Initializes the request in ways common to GET and POST requests.
		/// </summary>
		/// <param name="request">The request to initialize.</param>
		protected void InitializeRequest(HttpWebRequest request) {
			request.UserAgent = this.UserAgent;

			if (this.ReadWriteTimeout > TimeSpan.Zero) {
				request.ReadWriteTimeout = (int)this.ReadWriteTimeout.TotalMilliseconds;
			}

			if (this.Timeout > TimeSpan.Zero) {
				request.Timeout = (int)this.Timeout.TotalMilliseconds;
			}

			if (this.UseCompression) {
				request.Headers.Add("Accept-Encoding:gzip, deflate");
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			}
		}
	}
}
