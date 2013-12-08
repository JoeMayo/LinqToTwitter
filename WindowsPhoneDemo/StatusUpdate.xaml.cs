using System;
using System.Linq;
using System.Windows;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace WindowsPhoneDemo
{
    public partial class StatusUpdate : PhoneApplicationPage
    {
        public StatusUpdate()
        {
            InitializeComponent();

            TweetTextBox.Text = "Windows Phone Test, " + DateTime.Now.ToString() + " #linq2twitter";
        }

        private void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TweetTextBox.Text))
            {
                MessageBox.Show("Please enter text to tweet.");
            }

            ITwitterAuthorizer auth = SharedState.Authorizer;

            if (auth == null || !auth.IsAuthorized)
            {
                NavigationService.Navigate(new Uri("/OAuth.xaml", UriKind.Relative));
            }
            else
            {
                var twitterCtx = new TwitterContext(auth);

                twitterCtx.UpdateStatus(TweetTextBox.Text,
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
                                    updateResp.Exception.ToString(),
                                    updateResp.Message,
                                    MessageBoxButton.OK);
                                break;
                        }
                    }));
            }
        }
    }
}