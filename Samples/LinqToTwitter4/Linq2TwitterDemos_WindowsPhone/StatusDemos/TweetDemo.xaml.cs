using System;
using System.Linq;
using System.Windows;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace Linq2TwitterDemos_WindowsPhone.StatusDemos
{
    public partial class TweetDemo : PhoneApplicationPage
    {
        public TweetDemo()
        {
            InitializeComponent();
            TweetTextBox.Text = "Windows Phone Test, " + DateTime.Now.ToString() + " #linq2twitter";
        }

        async void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            IAuthorizer auth = SharedState.Authorizer;

            var twitterCtx = new TwitterContext(auth);

            decimal latitude = 37.78215m;
            decimal longitude = -122.40060m;

            Status tweet = await twitterCtx.TweetAsync(TweetTextBox.Text, latitude, longitude);

            MessageBox.Show(
                "User: " + tweet.User.ScreenNameResponse +
                ", Posted Status: " + tweet.Text,
                "Update Successfully Posted.",
                MessageBoxButton.OK);

            TweetTextBox.Text = "Windows Phone Test, " + DateTime.Now.ToString() + " #linq2twitter";
        }
    }
}