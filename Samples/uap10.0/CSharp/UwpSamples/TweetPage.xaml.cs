using System;
using LinqToTwitter;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpSamples
{
    /// <summary>
    /// Send a Tweet Sample.
    /// </summary>
    public sealed partial class TweetPage : Page
    {
        public TweetPage()
        {
            this.InitializeComponent();

            tweetText.Text = "Testing from a Universal Windows App: " + DateTime.Now;
        }

        private async void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            var authorizer = new UniversalAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                Callback = "http://github.com/JoeMayo/LinqToTwitter"
            };

            await authorizer.AuthorizeAsync();
            var ctx = new TwitterContext(authorizer);

            string userInput = tweetText.Text;
            Status tweet = await ctx.TweetAsync(userInput);

            ResponseTextBlock.Text = tweet.Text;

            await new MessageDialog("You Tweeted: " + tweet.Text, "Success!").ShowAsync();
        }
    }
}
