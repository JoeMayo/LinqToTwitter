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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using DotNetOpenAuth.OAuth;
using Kerr;

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
            : base(new DesktopConsumer(serviceProviderDescription, new WindowsCredentialStoreTokenManager()))
        {

            var inMemoryTokenManager = this.Consumer.TokenManager as WindowsCredentialStoreTokenManager;
            if (inMemoryTokenManager != null)
            {
                inMemoryTokenManager.SetAuthenticationTarget(this.AuthenticationTarget);
            }
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
        /// Gets a value indicating whether this authorization mechanism can immediately
        /// provide the user with access to his account without prompting (again)
        /// for his credentials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
        /// </value>
        public override bool CachedCredentialsAvailable
        {
            get { return Kerr.Credential.Exists(this.AuthenticationTarget, Kerr.CredentialType.Generic); }
        }

        /// <summary>
        /// Gets or sets the function that will ask the user for the verifier code (PIN).
        /// </summary>
        public Func<string> GetVerifier { get; set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        protected override string AccessToken
        {
            get { return this.TokenManager.AccessToken; }
        }

        /// <summary>
        /// Gets the token manager.
        /// </summary>
        private WindowsCredentialStoreTokenManager TokenManager
        {
            get { return (WindowsCredentialStoreTokenManager)base.Consumer.TokenManager; }
        }

        /// <summary>
        /// Clears the cached credentials, if any.
        /// </summary>
        public void ClearCachedCredentials()
        {
            Kerr.Credential.Delete(this.AuthenticationTarget, Kerr.CredentialType.Generic);
        }

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
