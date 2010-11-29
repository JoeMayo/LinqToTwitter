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
using System.Windows.Navigation;
using LinqToTwitter;

namespace LinqToTwitterSilverlightDemo.Views
{
    public partial class PublicStatusQuery : Page
    {
        public PublicStatusQuery()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser &&
                Application.Current.HasElevatedPermissions)
            {
                var twitterCtx = new TwitterContext();

                var result =
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.Public
                     select tweet)
                    .AsyncCallback(tweets =>
                        Dispatcher.BeginInvoke(() =>
                        {
                            var projectedTweets =
                               (from tweet in tweets
                                select new MyTweet
                                {
                                    ScreenName = tweet.User.Identifier.ScreenName,
                                    Tweet = tweet.Text
                                })
                               .ToList();

                            dataGrid1.ItemsSource = projectedTweets;
                        }))
                    .SingleOrDefault();
            }
            else
            {
                MessageBox.Show("Your app must be running out-of-browser and have elevated permissions.");
            }
        }

    }
}
