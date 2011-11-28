using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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

        private void PublicTimelineButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = new TwitterContext();

            (from tweet in ctx.Status
             where tweet.Type == StatusType.Public
             select tweet)
            .AsyncCallback(tweets =>
                Dispatcher.BeginInvoke(() =>
                {
                    var publicTweets =
                        (from tweet in tweets
                         select new PublicTweet
                         {
                             UserName = tweet.User.Identifier.ScreenName,
                             Message = tweet.Text,
                             ImageSource = tweet.User.ProfileImageUrl
                         })
                        .ToList();

                    PublicTweetListBox.ItemsSource = publicTweets;
                }))
            .SingleOrDefault();
        }
    }
}