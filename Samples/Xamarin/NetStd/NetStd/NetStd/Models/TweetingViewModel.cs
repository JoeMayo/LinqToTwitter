using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LinqToTwitter;
using Xamarin.Forms;

namespace NetStd.Models
{
    public class TweetingViewModel : INotifyPropertyChanged
    {
        List<Tweet> tweets;
        public List<Tweet> Tweets
        {
            get { return tweets; }
            set
            {
                if (tweets == value) return;

                tweets = value;
                OnPropertyChanged();
            }
        }

        public async Task InitTweetViewModel()
        {
            string consumerKey = "";
            string consumerSecret = "";

            var authSvc = DependencyService.Get<ILinqToTwitterAuthorizer>();
            IAuthorizer auth = authSvc.GetAuthorizer(consumerKey, consumerSecret);

            await auth.AuthorizeAsync();

            var ctx = new TwitterContext(auth);

            Search searchResponse = await
                (from search in ctx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "\"Twitter\""
                 select search)
                .SingleAsync();

            Tweets =
                (from tweet in searchResponse.Statuses
                 select new Tweet
                 {
                     StatusID = tweet.StatusID,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.Text,
                     ImageUrl = tweet.User.ProfileImageUrl
                 })
                .ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("Can't call OnPropertyChanged with a null property name.", propertyName);

            PropertyChangedEventHandler propChangedHandler = PropertyChanged;
            if (propChangedHandler != null)
                propChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
