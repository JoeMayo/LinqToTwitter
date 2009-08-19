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
using System.Web;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides OAuth authorization to Twitter client web applications.
    /// </summary>
    [Serializable]
    public class WebOAuthAuthorization : OAuthAuthorization
    {
        /// <summary>
        /// The access token.
        /// </summary>
        private string accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="accessToken">The access token, or null if the user doesn't have one yet.</param>
        public WebOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken)
            : this(tokenManager, accessToken, TwitterServiceDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="serviceProviderDescription">The service provider description.</param>
        public WebOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken, ServiceProviderDescription serviceProviderDescription) :
            base(new WebConsumer(serviceProviderDescription, tokenManager))
        {
            this.accessToken = accessToken;
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
            get { return this.AccessToken != null; }
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        protected override string AccessToken
        {
            get { return this.accessToken; }
        }

        /// <summary>
        /// Gets or sets the consumer.
        /// </summary>
        /// <value>The consumer.</value>
        protected new WebConsumer Consumer
        {
            get { return (WebConsumer)base.Consumer; }
        }

        /// <summary>
        /// Requests Twitter to authorize this app.
        /// </summary>
        /// <returns>The message that must be used to send the user to Twitter to authorize the app.</returns>
        public void BeginAuthorize()
        {
            this.Consumer.Channel.Send(this.Consumer.PrepareRequestUserAuthorization());
        }

        /// <summary>
        /// Exchanges an authorized request token for an access token.
        /// </summary>
        /// <returns>The newly acquired access token, or <c>null</c> if no authorization complete message was in the HTTP request.</returns>
        public string CompleteAuthorize()
        {
            var response = this.Consumer.ProcessUserAuthorization();
            if (response != null)
            {
                this.accessToken = response.AccessToken;
                this.ParseUserInfoFromAuthorizationResult(response.ExtraData);
                return response.AccessToken;
            }

            return null;
        }

        /// <summary>
        /// Invokes the entire authorization flow and blocks until it is complete.
        /// </summary>
        /// <returns>
        /// The extra data included in the last OAuth leg from Twitter that contains the user id and screen name.
        /// </returns>
        public override IDictionary<string, string> Authorize()
        {
            // This method should only be called by SignOn, and is only called then
            // if this component isn't already authorized.
            // We cannot complete authorization here as a single step because it requires a redirect.
            throw new InvalidOperationException("You must call BeginAuthorize and CompleteAuthorize on the WebOAuthAuthorization type itself first.");
        }
    }
}
