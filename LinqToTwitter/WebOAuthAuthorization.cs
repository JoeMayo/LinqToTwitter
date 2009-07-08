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
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides OAuth authorization to Twitter client web applications.
    /// </summary>
    public class WebOAuthAuthorization : OAuthAuthorization
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        public WebOAuthAuthorization()
            : this(TwitterServiceDescription)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="serviceProviderDescription">The service provider description.</param>
        public WebOAuthAuthorization(ServiceProviderDescription serviceProviderDescription) :
            base(new WebConsumer(serviceProviderDescription, new InMemoryTokenManager()))
        {
        }

        /// <summary>
        /// Gets or sets the consumer.
        /// </summary>
        /// <value>The consumer.</value>
        private new WebConsumer Consumer
        {
            get { return (WebConsumer)base.Consumer; }
        }

        /// <summary>
        /// Requests Twitter to authorize this app.
        /// </summary>
        /// <returns>The message that must be used to send the user to Twitter to authorize the app.</returns>
        public UserAuthorizationRequest BeginAuthorize()
        {
            return this.Consumer.PrepareRequestUserAuthorization();
        }

        /// <summary>
        /// Exchanges an authorized request token for an access token.
        /// </summary>
        /// <returns>The extra parameters Twitter included in its last response.</returns>
        public void CompleteAuthorize()
        {
            var response = this.Consumer.ProcessUserAuthorization();
            SaveCredentials();
            this.ParseUserInfoFromAuthorizationResult(response.ExtraData);
        }

        public override IDictionary<string, string> Authorize()
        {
            // This method should only be called by SignOn, and is only called then
            // if this component isn't already authorized.
            // We cannot complete authorization here as a single step because it requires a redirect.
            throw new InvalidOperationException("You must call BeginAuthorize and CompleteAuthorize on the WebOAuthAuthorization type itself first.");
        }
    }
}
