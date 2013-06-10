using System;
using System.Linq;
using System.Windows;
using LinqToTwitter;
using Microsoft.Phone.Controls;

namespace WindowsPhoneDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ITwitterAuthorizer auth = SharedState.Authorizer;

            if (auth == null || !auth.IsAuthorized)
            {
                NavigationService.Navigate(new Uri("/OAuth.xaml", UriKind.Relative));
            }
            else
            {
                var twitterCtx = new TwitterContext(auth, SharpGIS.WebRequestCreator.GZip);

                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == QueryTextBox.Text
                 select search)
                .MaterializedAsyncCallback(asyncResponse =>
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (asyncResponse.Status != TwitterErrorStatus.Success)
                        {
                            MessageBox.Show("Error during query: " + asyncResponse.Exception.Message);
                            return;
                        }

                        Search search = asyncResponse.State.SingleOrDefault();

                        var tweets =
                            (from status in search.Statuses
                             select new Tweet
                             {
                                 UserName = status.User.Identifier.ScreenName,
                                 Message = status.Text,
                                 ImageSource = status.User.ProfileImageUrl
                             })
                            .ToList();

                        SearchListBox.ItemsSource = tweets;
                    }));
            }
        }
    }
}