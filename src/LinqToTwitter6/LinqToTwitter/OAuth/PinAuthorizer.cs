using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class PinAuthorizer : AuthorizerBase, IAuthorizer
    {
        /// <summary>
        /// PIN-based authorization requires a 7-digit pin that is provided by Twitter.
        /// The user must copy that PIN and give it back to the program to use as a verifier
        /// in getting the final access token from Twitter.  You should write code (a lambda)
        /// that allows the user to provide this pin that this code will return.
        /// </summary>
        public Func<string> GetPin { get; set; }

        /// <summary>
        /// Action to redirect user to Twitter authorization page
        /// </summary>
        public Action<string> GoToTwitterAuthorization { get; set; }

        public PinAuthorizer()
            : base(false, AuthAccessType.NoChange, null) { }

        public PinAuthorizer(bool forceLogin)
            : base(forceLogin, AuthAccessType.NoChange, null) { }

        public PinAuthorizer(bool forceLogin, AuthAccessType accessType) 
            : base(forceLogin, accessType, null) { }

        public PinAuthorizer(bool forceLogin, AuthAccessType accessType, string preFillScreenName)
            : base(forceLogin, accessType, preFillScreenName) { }

        public async Task AuthorizeAsync()
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

            if (GetPin == null)
                throw new InvalidOperationException("You must provide an Func<string> delegate/lambda for GetPin.");

            await GetRequestTokenAsync("oob").ConfigureAwait(false);

            string authUrl = PrepareAuthorizeUrl(ForceLogin);
            GoToTwitterAuthorization(authUrl);

            string verifier = GetPin();

            var accessTokenParams = new Dictionary<string, string>();
            accessTokenParams.Add("oauth_verifier", verifier);
            await GetAccessTokenAsync(accessTokenParams).ConfigureAwait(false);
        }

        public async Task BeginAuthorizeAsync()
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

            await GetRequestTokenAsync("oob").ConfigureAwait(false);

            string authUrl = PrepareAuthorizeUrl(ForceLogin);
            GoToTwitterAuthorization(authUrl);
        }


        public async Task CompleteAuthorizeAsync(string pin)
        {
            var accessTokenParams = new Dictionary<string, string>();
            accessTokenParams.Add("oauth_verifier", pin);
            await GetAccessTokenAsync(accessTokenParams).ConfigureAwait(false);
        }
    }
}
