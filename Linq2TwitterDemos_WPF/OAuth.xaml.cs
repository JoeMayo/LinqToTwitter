using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LinqToTwitter;

namespace Linq2TwitterDemos_WPF
{
    /// <summary>
    /// Interaction logic for OAuth.xaml
    /// </summary>
    public partial class OAuth : Window
    {
        PinAuthorizer pinAuth = new PinAuthorizer();

        public OAuth()
        {
            InitializeComponent();
        }

        async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pinAuth = new PinAuthorizer
            {
                // Get the ConsumerKey and ConsumerSecret for your app and load them here.
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
                // Note: GetPin isn't used here because we've broken the authorization
                // process into two parts: begin and complete
                GoToTwitterAuthorization = pageLink => Dispatcher.BeginInvoke((Action)
                    (() => OAuthWebBrowser.Navigate(new Uri(pageLink, UriKind.Absolute))))
            };

            await pinAuth.BeginAuthorizeAsync();
        }

        async void SubmitPinButton_Click(object sender, RoutedEventArgs e)
        {
            await pinAuth.CompleteAuthorizeAsync(PinTextBox.Text);
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

            Close();
        }
    }
}
