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
using System.Linq;
using System.Text;
using System.Net;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;
using System.Collections.Specialized;
using Kerr;
using System.Xml.Linq;
using System.Xml;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides Twitter client authorization using the OAuth authorization protocol.
    /// Instantiate one of the <see cref="DesktopOAuthAuthorization"/> or <see cref="WebOAuthAuthorization"/>
    /// subclasses.
    /// </summary>
    public abstract class OAuthAuthorization : ITwitterAuthorization
    {
        /// <summary>
        /// The description of Twitter's OAuth protocol URIs.
        /// </summary>
        protected static readonly ServiceProviderDescription TwitterServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authorize", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAuthorization"/> class.
        /// </summary>
        public OAuthAuthorization(ConsumerBase consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            this.Consumer = consumer;

            var inMemoryTokenManager = consumer.TokenManager as InMemoryTokenManager;
            if (inMemoryTokenManager != null)
            {
                inMemoryTokenManager.SetAuthenticationTarget(this.AuthenticationTarget);
            }
        }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        /// <value>The consumer key.</value>
        public string ConsumerKey
        {
            get { return this.TokenManager.ConsumerKey; }
            set { this.TokenManager.ConsumerKey = value; }
        }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <value>The consumer secret.</value>
        public string ConsumerSecret
        {
            get { return this.TokenManager.ConsumerSecret; }
            set { this.TokenManager.ConsumerSecret = value; }
        }

        /// <summary>
        /// Gets the service description.
        /// </summary>
        /// <value>The service description.</value>
        internal ServiceProviderDescription ServiceDescription
        {
            get { return this.Consumer.ServiceProvider; }
        }

        /// <summary>
        /// Gets or sets the consumer.
        /// </summary>
        /// <value>The consumer.</value>
        protected ConsumerBase Consumer { get; private set; }

        /// <summary>
        /// Gets the token manager.
        /// </summary>
        /// <value>The token manager.</value>
        private InMemoryTokenManager TokenManager
        {
            get { return (InMemoryTokenManager)this.Consumer.TokenManager; }
        }

        #region ITwitterAuthorization Members

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }

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

        /// <summary>
        /// Gets the UserID that Twitter has assigned to the logged in user.
        /// </summary>
        /// <value>An integer number, represented as a string.</value>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the screenname of the user logged into Twitter.
        /// </summary>
        public string ScreenName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready to send authorized GET and POST requests.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthorized { get; private set; }

        /// <summary>
        /// Gets or sets the base Service URI of the Twitter service to authenticate to.
        /// </summary>
        public string AuthenticationTarget
        {
            get
            {
                return this.ServiceDescription.RequestTokenEndpoint.Location.AbsoluteUri;
            }

            set
            {
                // Setting this doesn't make sense, since there are several OAuth URLs that would need to change.
                throw new NotSupportedException("This property cannot be set on an OAuth authorization module.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this authorization mechanism can immediately
        /// provide the user with access to his account without prompting (again)
        /// for his credentials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
        /// </value>
        public bool CachedCredentialsAvailable
        {
            get { return Kerr.Credential.Exists(this.AuthenticationTarget, Kerr.CredentialType.Generic); }
        }

        /// <summary>
        /// Logs the user into the web site, prompting for credentials if necessary.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the authentication/authorization.</exception>
        public void SignOn()
        {
            IDictionary<string, string> lastArgs;

            if (!this.CachedCredentialsAvailable)
            {
                lastArgs = this.Authorize();
            }
            else
            {
                // Validate that the access code is still good
                lastArgs = ValidateLogin();
            }

            ParseUserInfoFromAuthorizationResult(lastArgs);
        }

        /// <summary>
        /// Parses the user info from the authorization result.
        /// </summary>
        /// <param name="lastArgs">The extra parameters Twitter included in the last authorization response.</param>
        protected void ParseUserInfoFromAuthorizationResult(IDictionary<string, string> lastArgs)
        {
            if (lastArgs == null)
            {
                throw new ArgumentNullException("lastArgs");
            }

            string screenName, userId;
            lastArgs.TryGetValue("screen_name", out screenName);
            lastArgs.TryGetValue("user_id", out userId);
            this.ScreenName = screenName;
            this.UserId = userId;
            this.IsAuthorized = true;
        }

        /// <summary>
        /// Where applicable, cancels session tokens (like an HTTP cookie), effectively logging the user off.
        /// </summary>
        public void SignOff()
        {
            this.IsAuthorized = false;
            this.UserId = null;
            this.ScreenName = null;
        }

        /// <summary>
        /// Clears the cached credentials, if any.
        /// </summary>
        public void ClearCachedCredentials()
        {
            Kerr.Credential.Delete(this.AuthenticationTarget, Kerr.CredentialType.Generic);
        }

        /// <summary>
        /// Prepares an authorized HTTP GET request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The arguments to include in the query string.</param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/> object that may be further customized.
        /// </returns>
        public HttpWebRequest Get(Uri requestUrl, IDictionary<string, string> args)
        {
            return this.Consumer.PrepareAuthorizedRequest(new MessageReceivingEndpoint(requestUrl, HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest), this.TokenManager.AccessToken);
        }

        /// <summary>
        /// Prepares an authorized HTTP POST request without sending a POST entity stream.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/> object that may be further customized.
        /// </returns>
        public HttpWebRequest Post(Uri requestUrl)
        {
            return this.Consumer.PrepareAuthorizedRequest(new MessageReceivingEndpoint(requestUrl, HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest), this.TokenManager.AccessToken);
        }

        /// <summary>
        /// Prepares and sends an authorized HTTP POST request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The parameters to include in the POST entity.  Must not be null.</param>
        /// <returns>The HTTP response.</returns>
        /// <exception cref="WebException">Thrown if the server returns an error.</exception>
        public HttpWebResponse Post(Uri requestUrl, IDictionary<string, string> args)
        {
            ((HttpWebRequest)WebRequest.Create(requestUrl)).ServicePoint.Expect100Continue = false;
            return (HttpWebResponse)this.Consumer.PrepareAuthorizedRequest(new MessageReceivingEndpoint(requestUrl, HttpDeliveryMethods.PostRequest), this.TokenManager.AccessToken).GetResponse();
        }

        #endregion

        /// <summary>
        /// Invokes the entire authorization flow and blocks until it is complete.
        /// </summary>
        /// <returns>The extra data included in the last OAuth leg from Twitter that contains the user id and screen name.</returns>
        public abstract IDictionary<string, string> Authorize();

        /// <summary>
        /// Saves the access token.
        /// </summary>
        [Obsolete("InMemoryTokenManager should do this implicitly.")]
        protected void SaveCredentials()
        {
            var cred = new Credential(
                AuthenticationTarget,
                CredentialType.Generic,
                this.TokenManager.AccessToken,
                this.TokenManager.GetTokenSecret(this.TokenManager.AccessToken).ToSecureString(),
                CredentialPersistence.LocalComputer,
                "OAuth access token and secret");
            cred.Save();
        }

        /// <summary>
        /// Verifies that a cached access token is still valid.
        /// </summary>
        /// <returns>The extra parameters Twitter included in the "yes, the access token is valid" response.</returns>
        private IDictionary<string, string> ValidateLogin()
        {
            var result = new Dictionary<string, string>();
            var request = Get(new Uri("http://twitter.com/account/verify_credentials.xml"), null);
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var xdoc = XDocument.Load(XmlReader.Create(responseStream));
                    result["user_id"] = xdoc.Element("user").Element("id").Value;
                    result["screen_name"] = xdoc.Element("user").Element("screen_name").Value;
                }
            }

            this.IsAuthorized = true;
            return result;
        }
    }
}
