using System;
using LinqToTwitter;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UwpSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
