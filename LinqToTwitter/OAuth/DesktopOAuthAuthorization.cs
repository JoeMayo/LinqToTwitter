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
using DotNetOpenAuth.OAuth.ChannelElements;
using Kerr;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides OAuth authorization to installed Twitter desktop applications.
    /// </summary>
    [Serializable]
    public class DesktopOAuthAuthorization : OAuthAuthorization
    {
        /// <summary>
        /// The request token that is being authorized.
        /// </summary>
        private string requestToken;

        /// <summary>
        /// The access token to use when we're using a custom token manager.
        /// </summary>
        private string accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopOAuthAuthorization"/> class.
        /// </summary>
        public DesktopOAuthAuthorization()
            : this(TwitterServiceDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopOAuthAuthorization"/> class
        /// that uses a custom token manager.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="accessToken">The access token.</param>
        public DesktopOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken)
            : this(tokenManager, accessToken, TwitterServiceDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopOAuthAuthorization"/> class
        /// that uses a custom token manager.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="accessToken">The access token.</param>
        public DesktopOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken, ServiceProviderDescription serviceProviderDescription)
            : base(new DesktopConsumer(serviceProviderDescription, tokenManager))
        {
            this.accessToken = accessToken;
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
        ///     <c>true</c> if cached credentials are available; otherwise, <c>false</c>.
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
        /// Gets or sets the consumer key.
        /// </summary>
        /// <value>Default value is the AppSetting stored as twitterConsumerKey.</value>
        public string ConsumerKey
        {
            get
            {
                return this.Consumer.TokenManager.ConsumerKey;
            }

            set
            {
                var credTokenManager = this.Consumer.TokenManager as WindowsCredentialStoreTokenManager;
                if (credTokenManager != null)
                {
                    credTokenManager.ConsumerKey = value;
                }
                else
                {
                    throw new InvalidOperationException("The ConsumerKey can only be set in this way when using the standard " + typeof(WindowsCredentialStoreTokenManager).Name + ".  For a custom class, set the ConsumerKey on that directly.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <value>Default value is the AppSetting stored as twitterConsumerSecret.</value>
        public string ConsumerSecret
        {
            get
            {
                return this.Consumer.TokenManager.ConsumerSecret;
            }

            set
            {
                var credTokenManager = this.Consumer.TokenManager as WindowsCredentialStoreTokenManager;
                if (credTokenManager != null)
                {
                    credTokenManager.ConsumerSecret = value;
                }
                else
                {
                    throw new InvalidOperationException("The ConsumerSecret can only be set in this way when using the standard " + typeof(WindowsCredentialStoreTokenManager).Name + ".  For a custom class, set the ConsumerSecret on that directly.");
                }
            }
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public override string AccessToken
        {
            get
            {
                var credTokenManager = this.TokenManager as WindowsCredentialStoreTokenManager;
                if (credTokenManager != null)
                {
                    return credTokenManager.AccessToken;
                }
                else
                {
                    return this.accessToken;
                }
            }
        }

        /// <summary>
        /// Gets the token manager.
        /// </summary>
        private IConsumerTokenManager TokenManager
        {
            get { return base.Consumer.TokenManager; }
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
