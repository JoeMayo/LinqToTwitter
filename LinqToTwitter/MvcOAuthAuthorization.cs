//-----------------------------------------------------------------------
// <copyright file="Utilities.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using System;

namespace LinqToTwitter
{
    [Serializable]
    public class MvcOAuthAuthorization : WebOAuthAuthorization
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="tokenManager">The token manager.</param>
        /// <param name="accessToken">The access token, or null if the user doesn't have one yet.</param>
        public MvcOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken)
            : base(tokenManager, accessToken)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOAuthAuthorization"/> class.
        /// </summary>
        /// <param name="serviceProviderDescription">The service provider description.</param>
        public MvcOAuthAuthorization(IConsumerTokenManager tokenManager, string accessToken, ServiceProviderDescription serviceProviderDescription) :
            base(tokenManager, accessToken, serviceProviderDescription)
        {
        }

        /// <summary>
        /// Requests Twitter to authorize this app.
        /// </summary>
        /// <returns>The message that must be used to send the user to Twitter to authorize the app.</returns>
        public new ActionResult BeginAuthorize()
        {
            return this.Consumer.Channel.PrepareResponse(this.Consumer.PrepareRequestUserAuthorization()).AsActionResult();
        }
    }
}
