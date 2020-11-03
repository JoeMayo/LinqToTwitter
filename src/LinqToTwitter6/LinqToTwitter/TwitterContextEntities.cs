using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        ///// <summary>
        ///// enables access to Twitter account information, such as Verify Credentials and Rate Limit Status
        ///// </summary>
        //public TwitterQueryable<Account> Account
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Account>(this);
        //    }
        //}

        ///// <summary>
        ///// Enables access to Twitter account activity information, such as listing webhooks and showing subscriptions.
        ///// </summary>
        //public TwitterQueryable<AccountActivity> AccountActivity
        //{
        //    get
        //    {
        //        return new TwitterQueryable<AccountActivity>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter blocking information, such as Exists, Blocks, and IDs
        ///// </summary>
        //public TwitterQueryable<Blocks> Blocks
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Blocks>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Direct Message Events, supporting Twitter chatbots
        ///// </summary>
        //public TwitterQueryable<DirectMessageEvents> DirectMessageEvents
        //{
        //    get
        //    {
        //        return new TwitterQueryable<DirectMessageEvents>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter Favorites
        ///// </summary>
        //public TwitterQueryable<Favorites> Favorites
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Favorites>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter Friendship info
        ///// </summary>
        //public TwitterQueryable<Friendship> Friendship
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Friendship>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter Geo info
        ///// </summary>
        //public TwitterQueryable<Geo> Geo
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Geo>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter Help info
        ///// </summary>
        //public TwitterQueryable<Help> Help
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Help>(this);
        //    }
        //}

        /// <summary>
        /// Enables access to media commands, like STATUS (Twitter API v1)
        /// </summary>
        public TwitterQueryable<Media> Media
        {
            get
            {
                return new TwitterQueryable<Media>(this);
            }
        }

        ///// <summary>
        ///// Enables access to muted users
        ///// </summary>
        //public TwitterQueryable<Mute> Mute
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Mute>(this);
        //    }
        //}

        ///// <summary>
        ///// enables access to Twitter List info
        ///// </summary>
        //public TwitterQueryable<List> List
        //{
        //    get
        //    {
        //        return new TwitterQueryable<List>(this);
        //    }
        //}

        /// <summary>
        /// enables access to Raw Query Extensibility (All Twitter API versions)
        /// </summary>
        public TwitterQueryable<Raw> RawQuery => new TwitterQueryable<Raw>(this);

        ///// <summary>
        ///// enables access to Twitter Saved Searches
        ///// </summary>
        //public TwitterQueryable<SavedSearch> SavedSearch
        //{
        //    get
        //    {
        //        return new TwitterQueryable<SavedSearch>(this);
        //    }
        //}

        /// <summary>
        /// enables access to Twitter Search to query tweets (Twitter API v1)
        /// </summary>
        public TwitterQueryable<Search> Search => new TwitterQueryable<Search>(this);

        /// <summary>
        /// enables access to Twitter Search v2 to query tweets (Twitter API v2)
        /// </summary>
        public TwitterQueryable<TwitterSearch> TwitterSearch => new TwitterQueryable<TwitterSearch>(this);

        /// <summary>
        /// enables access to Twitter Status messages (Twitter API v1)
        /// </summary>
        public TwitterQueryable<Status> Status
        {
            get
            {
                return new TwitterQueryable<Status>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter streams
        /// </summary>
        public TwitterQueryable<Streaming> Streaming
        {
            get
            {
                return new TwitterQueryable<Streaming>(this);
            }
        }

        ///// <summary>
        ///// enables access to Twitter Trends, such as Trend, Current, Daily, and Weekly
        ///// </summary>
        //public TwitterQueryable<Trend> Trends
        //{
        //    get
        //    {
        //        return new TwitterQueryable<Trend>(this);
        //    }
        //}

        /// <summary>
        /// enables access to Twitter Tweets lookup (Twitter API v2)
        /// </summary>
        public TwitterQueryable<TweetQuery> Tweets => new TwitterQueryable<TweetQuery>(this);

        /// <summary>
        /// enables access to Twitter User lookup (Twitter API v2)
        /// </summary>
        public TwitterQueryable<TwitterUserQuery> TwitterUser => new TwitterQueryable<TwitterUserQuery>(this);

        /// <summary>
        /// enables access to Twitter User messages, such as Friends and Followers (Twitter API v1)
        /// </summary>
        public TwitterQueryable<User> User
        {
            get
            {
                return new TwitterQueryable<User>(this);
            }
        }

        ///// <summary>
        ///// enables access to Twitter Welcome messages
        ///// </summary>
        //public TwitterQueryable<WelcomeMessage> WelcomeMessage
        //{
        //    get
        //    {
        //        return new TwitterQueryable<LinqToTwitter.WelcomeMessage>(this);
        //    }
        //}
    }
}
