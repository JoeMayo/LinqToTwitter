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

/// <summary>
/// Stores request and access tokens in memory and an instance of this class should
/// be stored in ASP.NET application state.
/// This is good for sample use ONLY.  Real web applications should store these tokens in a database.
/// </summary>
public class InMemoryTokenManager : IConsumerTokenManager
{
    private Dictionary<string, string> tokensAndSecrets = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryTokenManager"/> class.
    /// </summary>
    public InMemoryTokenManager()
    {
    }

    /// <summary>
    /// Gets the consumer key.
    /// </summary>
    public string ConsumerKey
    {
        get { return ConfigurationManager.AppSettings["twitterConsumerKey"]; }
    }

    /// <summary>
    /// Gets the consumer secret.
    /// </summary>
    public string ConsumerSecret
    {
        get { return ConfigurationManager.AppSettings["twitterConsumerSecret"]; }
    }

    #region ITokenManager Members

    public string GetTokenSecret(string token)
    {
        return this.tokensAndSecrets[token];
    }

    public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
    {
        this.tokensAndSecrets[response.Token] = response.TokenSecret;
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

    public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
    {
        this.tokensAndSecrets.Remove(requestToken);
        this.tokensAndSecrets[accessToken] = accessTokenSecret;
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
