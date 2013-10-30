using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class AspNetAuthorizer : AuthorizerBase, IAuthorizer
    {
        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public Uri Callback { get; set; }

        /// <summary>
        /// This is a hook where you can assign
        /// a lambda to perform the technology
        /// specific redirection action.
        /// 
        /// The string passed as the lambda paramter
        /// is the Twitter authorization URL.
        /// </summary>
        public Action<string> GoToTwitterAuthorization { get; set; }

        public AspNetAuthorizer()
            : base(false, AuthAccessType.NoChange, null) { }

        public AspNetAuthorizer(bool forceLogin)
            : base(forceLogin, AuthAccessType.NoChange, null) { }

        public AspNetAuthorizer(bool forceLogin, AuthAccessType accessType) 
            : base(forceLogin, accessType, null) { }

        public AspNetAuthorizer(bool forceLogin, AuthAccessType accessType, string preFillScreenName)
            : base(forceLogin, accessType, preFillScreenName) { }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public async Task AuthorizeAsync()
        {
            throw new InvalidOperationException("For ASP.NET apps, you should use BeginAuthorization and CompleteAuthorization instead. Please visit the LINQ to Twitter documentation and samples for examples on how to do this.");
        }

        /// <summary>
        /// First part of the authorization sequence that:
        /// 1. Obtains a request token and then
        /// 2. Redirects to the Twitter authorization page
        /// </summary>
        /// <param name="callback">This is where you want Twitter to redirect to after authorization</param>
        public async Task BeginAuthorizeAsync()
        {
            await BeginAuthorizeAsync(Callback);
        }

        /// <summary>
        /// First part of the authorization sequence that:
        /// 1. Obtains a request token and then
        /// 2. Redirects to the Twitter authorization page
        /// </summary>
        public async Task BeginAuthorizeAsync(Uri callback)
        {
            if (CredentialStore == null)
                throw new NullReferenceException(
                    "The authorization process requires a minimum of ConsumerKey and ConsumerSecret tokens. " +
                    "You must assign the CredentialStore property (with tokens) before calling AuthorizeAsync().");

            if (CredentialStore.HasAllCredentials()) return;

            if (string.IsNullOrWhiteSpace(CredentialStore.ConsumerKey) || string.IsNullOrWhiteSpace(CredentialStore.ConsumerSecret))
                throw new ArgumentException("You must populate CredentialStore with ConsumerKey and ConsumerSecret tokens before calling AuthorizeAsync.", "CredentialStore");

            if (GoToTwitterAuthorization == null)
                throw new InvalidOperationException("You must provide an Action<string> delegate/lambda for GoToTwitterAuthorization.");

            await GetRequestTokenAsync(callback.ToString());

            string authUrl = PrepareAuthorizeUrl(ForceLogin);
            GoToTwitterAuthorization(authUrl);
        }

        /// <summary>
        /// After the user Authorizes the app, Twitter will 
        /// redirect to the responseUrl url, provided during 
        /// BeginAuthorization. When redirecting, Twitter will 
        /// also provide oauth_verifier and oauth_token 
        /// parameters. This method uses those parameters to 
        /// request an access token, which is used automatically
        /// by LINQ to Twitter when executing queries.
        /// </summary>
        /// <param name="responseUrl">
        /// URL that Twitter redirected to after authorization.
        /// We need this because it contains important parameters
        /// we need to finish the OAuth process.
        /// </param>
        /// <returns>True if successful</returns>
        public async Task CompleteAuthorizeAsync(Uri responseUrl)
        {
            if (responseUrl == null)
                throw new ArgumentNullException("responseUrl", "You must pass in the callback that Twitter returned after authentication.");

            if (CredentialStore.HasAllCredentials()) return;

            string pin = ParseVerifierFromResponseUrl(responseUrl.ToString());

            var accessTokenParams = new Dictionary<string, string>();
            accessTokenParams.Add("oauth_verifier", pin);
            await GetAccessTokenAsync(accessTokenParams);
        }
    }
}
