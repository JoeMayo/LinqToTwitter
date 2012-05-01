using System;
using System.Linq;
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
            var ctx = new TwitterContext();

            (from search in ctx.Search
             where search.Type == SearchType.Search &&
                   search.Query == QueryTextBox.Text
             select search)
            .AsyncCallback(response =>
                Dispatcher.BeginInvoke(() =>
                {
                    var publicTweets =
                        (from result in response.Single().Results
                         select new PublicTweet
                         {
                             UserName = result.FromUser,
                             Message = result.Text,
                             ImageSource = result.ProfileImageUrl
                         })
                        .ToList();

                    SearchListBox.ItemsSource = publicTweets;
                }))
            .SingleOrDefault();
        }
    }
}