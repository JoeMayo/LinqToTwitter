using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Linq2TwitterDemos_WPF.ViewModels;
using LinqToTwitter;

namespace Linq2TwitterDemos_WPF.StatusDemos
{
    /// <summary>
    /// Interaction logic for HomeTimelineWindow.xaml
    /// </summary>
    public partial class HomeTimelineWindow : Window
    {
        public HomeTimelineWindow()
        {
            InitializeComponent();

            DataContext = new TweetViewModel();
        }

        async void Window_Loaded(object sender, RoutedEventArgs e)
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
