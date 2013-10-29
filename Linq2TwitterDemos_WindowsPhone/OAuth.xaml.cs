using System;
using System.Linq;
using System.Threading;
using System.Windows;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace Linq2TwitterDemos_WindowsPhone
{
    public partial class OAuth : PhoneApplicationPage
    {
        PinAuthorizer pinAuth = new PinAuthorizer();

        public OAuth()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            pinAuth = new PinAuthorizer
            {
                // Get the ConsumerKey and ConsumerSecret for your app and load them here.
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                // Note: GetPin isn't used here because we've broken the authorization
                // process into two parts: begin and complete
                GoToTwitterAuthorization = pageLink => Dispatcher.BeginInvoke(
                    () => OAuthWebBrowser.Navigate(new Uri(pageLink, UriKind.Absolute)))
            };

            await pinAuth.BeginAuthorizeAsync();
        }

        void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            pinAuth.CompleteAuthorizeAsync(PinTextBox.Text);
            SharedState.Authorizer = pinAuth;

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
            //
            //var credentials = pinAuth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            NavigationService.GoBack();
        }
    }
}