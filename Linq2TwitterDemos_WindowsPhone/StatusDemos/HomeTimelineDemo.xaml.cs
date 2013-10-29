using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Linq2TwitterDemos_WindowsPhone.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LinqToTwitter;

namespace Linq2TwitterDemos_WindowsPhone.StatusDemos
{
    public partial class HomeTimelineDemo : PhoneApplicationPage
    {
        public HomeTimelineDemo()
        {
            InitializeComponent();

            DataContext = new TweetViewModel();
        }

        async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select new Tweet
                 {
                     ImageSource = tweet.User.ProfileImageUrl,
                     UserName = tweet.User.ScreenNameResponse,
                     Message = tweet.Text
                 })
                .ToListAsync();

            var tweetCollection = (DataContext as TweetViewModel).Tweets;
            tweetCollection.Clear();
            tweets.ForEach(tweet => tweetCollection.Add(tweet));
        }
    }
}