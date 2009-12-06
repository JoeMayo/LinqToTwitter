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
using System.Configuration;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Kerr;

namespace LinqToTwitter
{
    /// <summary>
    /// A consumer token manager that stores a single access token in the Windows credential store.
    /// </summary>
    /// <remarks>
    /// The consumer key and secret must be kept in the the application .config file in appSettings
    /// under twitterConsumerKey and twitterConsumerSecret.
    /// </remarks>
    [Serializable]
    internal class WindowsCredentialStoreTokenManager : IConsumerTokenManager
    {
        /// <summary>
        /// The request token, which may be unauthorized or authorized depending on the state in the OAuth flow.
        /// </summary>
        /// <value>
        /// <c>A token assigned by the Service Provider; 
        /// or <c>null</c> if the flow hasn't started or an access token has already been obtained.</c>
        /// </value>
        private string requestToken;

        /// <summary>
        /// The request token secret, or <c>null</c> if <see cref="requestToken"/> is null.
        /// </summary>
        private string requestTokenSecret;

        /// <summary>
        /// The access token.
        /// </summary>
        private string accessToken;

        /// <summary>
        /// The access token secret.
        /// </summary>
        private string accessTokenSecret;

        /// <summary>
        /// The credential store for the access token and secret.
        /// </summary>
        private Credential credential;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsCredentialStoreTokenManager"/> class.
        /// </summary>
        internal WindowsCredentialStoreTokenManager()
        {
            this.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
            this.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
        }

        /// <summary>
        /// Gets the consumer key.
        /// </summary>
        /// <value>Default value is the AppSetting stored as twitterConsumerKey.</value>
        public string ConsumerKey { get; internal set; }

        /// <summary>
        /// Gets the consumer secret.
        /// </summary>
        /// <value>Default value is the AppSetting stored as twitterConsumerSecret.</value>
        public string ConsumerSecret { get; internal set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken
        {
            get { return this.accessToken; }
        }

        /// <summary>
        /// Sets the authentication target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetAuthenticationTarget(string target)
        {
            this.credential = new Credential(target, CredentialType.Generic);
            if (Credential.Exists(target, CredentialType.Generic))
            {
                this.credential.Load();
                this.accessToken = credential.UserName;
                this.accessTokenSecret = credential.Password.ToUnsecureString();
            }
        }

        #region ITokenManager Members

        /// <summary>
        /// Gets the Token Secret given a request or access token.
        /// </summary>
        /// <param name="token">The request or access token.</param>
        /// <returns>
        /// The secret associated with the given token.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Thrown if the secret cannot be found for the given token.</exception>
        public string GetTokenSecret(string token)
        {
            if (token == this.requestToken)
            {
                return this.requestTokenSecret;
            }
            else if (token == this.accessToken)
            {
                return this.accessTokenSecret;
            }
            else
            {
                throw new ArgumentOutOfRangeException("token");
            }
        }

        /// <summary>
        /// Stores a newly generated unauthorized request token, secret, and optional
        /// application-specific parameters for later recall.
        /// </summary>
        /// <param name="request">The request message that resulted in the generation of a new unauthorized request token.</param>
        /// <param name="response">The response message that includes the unauthorized request token.</param>
        /// <exception cref="T:System.ArgumentException">Thrown if the consumer key is not registered, or a required parameter was not found in the parameters collection.</exception>
        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            this.requestToken = response.Token;
            this.requestTokenSecret = response.TokenSecret;
        }

        /// <summary>
        /// Checks whether a given request token has already been authorized
        /// by some user for use by the Consumer that requested it.
        /// </summary>
        /// <param name="requestToken">The Consumer's request token.</param>
        /// <returns>
        /// True if the request token has already been fully authorized by the user
        /// who owns the relevant protected resources.  False if the token has not yet
        /// been authorized, has expired or does not exist.
        /// </returns>
        public bool IsRequestTokenAuthorized(string requestToken)
        {
            // Only needed by Service Providers.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a request token and its associated secret and stores a new access token and secret.
        /// </summary>
        /// <param name="consumerKey">The Consumer that is exchanging its request token for an access token.</param>
        /// <param name="requestToken">The Consumer's request token that should be deleted/expired.</param>
        /// <param name="accessToken">The new access token that is being issued to the Consumer.</param>
        /// <param name="accessTokenSecret">The secret associated with the newly issued access token.</param>
        /// <remarks>
        /// Any scope of granted privileges associated with the request token from the
        /// original call to <see cref="M:DotNetOpenAuth.OAuth.ChannelElements.ITokenManager.StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest,DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage)"/> should be carried over
        /// to the new Access Token.
        /// </remarks>
        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            this.requestToken = null;
            this.requestTokenSecret = null;
            this.accessToken = accessToken;
            this.accessTokenSecret = accessTokenSecret;

            if (credential != null)
            {
                credential.UserName = this.accessToken;
                credential.Password = accessTokenSecret.ToSecureString();
                credential.Save();
            }
        }

        /// <summary>
        /// Classifies a token as a request token or an access token.
        /// </summary>
        /// <param name="token">The token to classify.</param>
        /// <returns>Request or Access token, or invalid if the token is not recognized.</returns>
        public TokenType GetTokenType(string token)
        {
            // Only needed by Service Providers
            throw new NotImplementedException();
        }

        #endregion
    }
}