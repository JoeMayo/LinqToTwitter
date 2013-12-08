using System;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Navigation;
using LinqToTwitter;

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class HomeTimelineQuery : Page
    {
        private TwitterContext twitterCtx = null;
        private PinAuthorizer pinAuth = null;

        public HomeTimelineQuery()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Uncomment the code below if you need to use XAuth. Generally, XAuth isn't available unless you specifically
            // justify using it with Twitter: http://dev.twitter.com/pages/xauth. You should use OAuth instead.  However,
            // LINQ to Twitter supports XAuth if you're one of the rare cases that Twitter gives permission to.

            //DoXAuth();
            //return;

            if (Application.Current.IsRunningOutOfBrowser &&
                Application.Current.HasElevatedPermissions)
            {
                DoPinAuth(); 
            }
            else
            {
                DoWebAuth();
            }
        }

        private void DoWebAuth()
        {
            WebBrowser.Visibility = Visibility.Collapsed;
            PinPanel.Visibility = Visibility.Collapsed;

            var auth = new SilverlightAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                PerformRedirect = authUrl => 
                    Dispatcher.BeginInvoke(() => HtmlPage.Window.Navigate(new Uri(authUrl)))
            };

            Uri url = HtmlPage.Document.DocumentUri;

            auth.CompleteAuthorize(url, resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    // you can save these in the db and add logic
                    // to look for credentials from db before
                    // doing Oauth dance.
                    var oauthToken = auth.Credentials.OAuthToken;
                    var accessToken = auth.Credentials.AccessToken;

                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            FriendsPanel.Visibility = Visibility.Visible;
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

            if (!auth.IsAuthorized && !auth.IsAuthorizing)
            {
                auth.BeginAuthorize(url, resp =>
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
            }

            twitterCtx = new TwitterContext(auth);
        }

        private void DoPinAuth()
        {
            pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                UseCompression = true,
                GoToTwitterAuthorization = pageLink =>
                    Dispatcher.BeginInvoke(() => WebBrowser.Navigate(new Uri(pageLink)))
            };

            pinAuth.BeginAuthorize(resp =>
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

            twitterCtx = new TwitterContext(pinAuth);
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            string pin = PinTextBox.Text;

            pinAuth.CompleteAuthorize(
                PinTextBox.Text,
                completeResp => Dispatcher.BeginInvoke(() =>
                {
                    switch (completeResp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            FriendsPanel.Visibility = Visibility.Visible;
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

        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            (from tweet in twitterCtx.Status
             where tweet.Type == StatusType.Home
             select tweet)
            .AsyncCallback(tweets =>
                Dispatcher.BeginInvoke(() =>
                {
                    var projectedTweets =
                        (from tweet in tweets
                        select new MyTweet
                        {
                            ScreenName = tweet.User.Identifier.ScreenName,
                            Tweet = tweet.Text
                        })
                        .ToList();

                    FriendsDataGrid.ItemsSource = projectedTweets;
                }))
            .SingleOrDefault();
        }

        private void DoXAuth()
        {
            WebBrowser.Visibility = Visibility.Collapsed;
            PinPanel.Visibility = Visibility.Collapsed;

            var auth = new XAuthAuthorizer
            {
                Credentials = new XAuthCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = "",
                    UserName = "",
                    Password = ""
                }
            };

            auth.BeginAuthorize(resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            FriendsPanel.Visibility = Visibility.Visible;
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

            twitterCtx = new TwitterContext(auth);
        }
    }
}
