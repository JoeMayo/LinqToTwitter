using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace WindowsPhoneDemo
{
    public partial class OAuth : PhoneApplicationPage
    {
        PinAuthorizer pinAuth;

        public OAuth()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
            OAuthWebBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(OAuthWebBrowser_LoadCompleted);
        }

        void OAuthWebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            EnterPinTextBlock.Visibility = Visibility.Visible;
            PinTextBox.IsEnabled = true;
            AuthenticateButton.IsEnabled = true;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials 
                { 
                    ConsumerKey = "", 
                    ConsumerSecret = "" 
                },
                UseCompression = true,
                GoToTwitterAuthorization = pageLink => Dispatcher.BeginInvoke(() => OAuthWebBrowser.Navigate(new Uri(pageLink, UriKind.Absolute)))
            };

            this.pinAuth.BeginAuthorize(resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                resp.Exception.ToString(),
                                resp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));

            //
            // comment out the code above and uncomment this code to use SingleUserAuthorizer
            //

            //var auth = new SingleUserAuthorizer
            //{
            //    Credentials = new SingleUserInMemoryCredentials
            //    {
            //        ConsumerKey = "", // twitter Consumer key
            //        ConsumerSecret = "", // twitter Consumer secret
            //        TwitterAccessToken = "", // twitter Access token
            //        TwitterAccessTokenSecret = "" // twitter Access token secret
            //    }
            //};

            //SharedState.Authorizer = auth;

            //NavigationService.GoBack();
        }

        private void AuthenticateButton_Click(object sender, RoutedEventArgs e)
        {
            pinAuth.CompleteAuthorize(
                PinTextBox.Text,
                completeResp => Dispatcher.BeginInvoke(() =>
                {
                    switch (completeResp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            SharedState.Authorizer = pinAuth;
                            NavigationService.GoBack();
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                completeResp.Exception.ToString(),
                                completeResp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));
        }
    }
}