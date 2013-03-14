using LinqToTwitter.OAuth;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public class WinRtAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public Uri Callback { get; set; }

        /// <summary>
        /// Don't use - call AuthorizeAsync instead.
        /// </summary>
        public void Authorize()
        {
            throw new InvalidOperationException("Please call AuthorizeAsync instead.");
        }

        /// <summary>
        /// Perform Authorization
        /// </summary>
        public async Task<WinRtAuthorizer> AuthorizeAsync()
        {
            await LoadCredentials();

            dynamic configuration = new ExpandoObject();
            configuration.TwitterClientId = Credentials.ConsumerKey;
            configuration.TwitterClientSecret = Credentials.ConsumerSecret;
            configuration.TwitterRedirectUrl = Callback.ToString();

            var twitAuthentication = new TwitterAuthProvider();
            twitAuthentication.Configure(configuration);

            var user = await twitAuthentication.AuthenticateAsync();
            if (twitAuthentication.OAuthToken != null && twitAuthentication.OAuthTokenSecret != null)
            {
                OAuthTwitter.OAuthToken = twitAuthentication.OAuthToken;
                OAuthTwitter.OAuthTokenSecret = twitAuthentication.OAuthTokenSecret;
                Credentials.ScreenName = user.UserName;
                Credentials.UserId = user.Id;
                Credentials.OAuthToken = twitAuthentication.OAuthToken;
                Credentials.AccessToken = twitAuthentication.OAuthTokenSecret;

                await SaveCredentials();
            }

            return this;
        }

        async Task LoadCredentials()
        {
            var creds = Credentials as IAsyncOAuthCredentials;

            if (creds == null)
                (Credentials as IWinRtSettingsCredentials).LoadCredentialsFromSettings();
            else
                await creds.LoadCredentialsFromStorageFileAsync();
        }

        async Task SaveCredentials()
        {
            var creds = Credentials as IAsyncOAuthCredentials;

            if (creds == null)
                (Credentials as IWinRtSettingsCredentials).SaveCredentialsToSettings();
            else
                await creds.SaveCredentialsToStorageFileAsync();
        }
    }
}
