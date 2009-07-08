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
using System.Collections.Specialized;
using System.Globalization;
using DotNetOpenAuth.OAuth;
using System.Diagnostics;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides OAuth authorization to installed Twitter desktop applications.
    /// </summary>
    public class DesktopOAuthAuthorization : OAuthAuthorization
    {
        /// <summary>
        /// The request token that is being authorized.
        /// </summary>
        private string requestToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopOAuthAuthorization"/> class.
        /// </summary>
        public DesktopOAuthAuthorization()
            : this(TwitterServiceDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        public DesktopOAuthAuthorization(ServiceProviderDescription serviceProviderDescription)
            : base(new DesktopConsumer(serviceProviderDescription, new InMemoryTokenManager()))
        {
        }

        /// <summary>
        /// Gets or sets the consumer.
        /// </summary>
        /// <value>The consumer.</value>
        private new DesktopConsumer Consumer
        {
            get { return (DesktopConsumer)base.Consumer; }
        }

        /// <summary>
        /// Gets or sets the function that will ask the user for the verifier code (PIN).
        /// </summary>
        public Func<string> GetVerifier { get; set; }

        /// <summary>
        /// Generates the URI to direct the user to in order to complete authorization.
        /// </summary>
        public Uri BeginAuthorize()
        {
            return this.Consumer.RequestUserAuthorization(null, null, out this.requestToken);
        }

        /// <summary>
        /// Exchanges an authorized request token for an access token.
        /// </summary>
        /// <returns>The extra parameters Twitter included in its last response.</returns>
        /// <remarks>The <see cref="GetVerifier"/> property MUST be set before this call.</remarks>
        public IDictionary<string, string> CompleteAuthorize()
        {
            if (this.GetVerifier == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}.GetVerifier must be set first.", this.GetType().Name));
            }

            var response = this.Consumer.ProcessUserAuthorization(this.requestToken, this.GetVerifier());
            SaveCredentials();

            return response.ExtraData;
        }

        /// <summary>
        /// Invokes the entire authorization flow and blocks until it is complete.
        /// </summary>
        /// <returns>
        /// The extra data included in the last OAuth leg from Twitter that contains the user id and screen name.
        /// </returns>
        /// <remarks>The <see cref="GetVerifier"/> property MUST be set before this call.</remarks>
        public override IDictionary<string, string> Authorize()
        {
            if (this.GetVerifier == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0}.GetVerifier must be set first.", this.GetType().Name));
            }

            Process.Start(this.BeginAuthorize().AbsoluteUri);
            return this.CompleteAuthorize();
        }
    }
}
