using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Navigation;
using LinqToTwitter;

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class StatusUpdate : Page
    {
        private TwitterContext m_twitterCtx = null;
        private PinAuthorizer m_pinAuth = null;

        public StatusUpdate()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        private void DoPinAuth()
        {
            m_pinAuth = new PinAuthorizer
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

            m_pinAuth.BeginAuthorize(resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                resp.Error.ToString(),
                                resp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));

            m_twitterCtx = new TwitterContext(m_pinAuth, "https://api.twitter.com/1/", "https://search.twitter.com/");
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
                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            UpdatePanel.Visibility = Visibility.Visible;
                            TweetTextBox.Text = "Silverlight Web Test, " + DateTime.Now.ToString() + " #linqtotwitter";
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                resp.Error.ToString(),
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
                                    resp.Error.ToString(),
                                    resp.Message,
                                    MessageBoxButton.OK);
                                break;
                        }
                    }));
            }

            m_twitterCtx = new TwitterContext(auth);
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            m_pinAuth.CompleteAuthorize(
                PinTextBox.Text,
                completeResp => Dispatcher.BeginInvoke(() =>
                {
                    switch (completeResp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            UpdatePanel.Visibility = Visibility.Visible;
                            TweetTextBox.Text = "Silverlight OOB Test, " + DateTime.Now.ToString() + " #linqtotwitter";
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                completeResp.Error.ToString(),
                                completeResp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            m_twitterCtx.UpdateStatus(TweetTextBox.Text,
                updateResp => Dispatcher.BeginInvoke(() =>
                {
                    switch (updateResp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            Status tweet = updateResp.State;
                            User user = tweet.User;
                            UserIdentifier id = user.Identifier;
                            MessageBox.Show(
                                "User: " + id.ScreenName +
                                ", Posted Status: " + tweet.Text,
                                "Update Successfully Posted.",
                                MessageBoxButton.OK);
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                updateResp.Error.ToString(),
                                updateResp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));
        }
    }
}
