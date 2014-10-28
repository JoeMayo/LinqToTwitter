using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WindowsStore.DataModel
{
    class TweetViewModel : INotifyPropertyChanged
    {
        public List<Tweet> Tweets { get; set; }

        public TweetViewModel()
        {
            RefreshCommand = new TwitterCommand<object>(OnRefresh);
        }

        public TwitterCommand<object> RefreshCommand { get; set; }

        async void OnRefresh(object obj)
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var timelineResponse =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select tweet)
                .ToListAsync();

            Tweets =
                (from tweet in timelineResponse
                 select new Tweet
                 {
                     Name = tweet.User.ScreenNameResponse,
                     Text = tweet.Text,
                     ImageUrl = tweet.User.ProfileImageUrl
                 })
                .ToList();

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tweets"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
