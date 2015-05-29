using System.Collections.ObjectModel;
using System.Linq;
using LinqToTwitter;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpSamples
{
    /// <summary>
    /// Twitter API Search Demo
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();

            searchText.Text = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter";
        }

        async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var authorizer = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                }
            };

            await authorizer.AuthorizeAsync();
            var ctx = new TwitterContext(authorizer);

            string searchString = searchText.Text;

            var searchResponse =
                await
                (from search in ctx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchString
                 select search)
                .SingleOrDefaultAsync();

            var tweets =
                (from tweet in searchResponse.Statuses
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.Text
                 })
                .ToList();

            tweetListView.ItemsSource = new ObservableCollection<TweetViewModel>(tweets);
        }
    }
}
