using System;
using System.Linq;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// enables access to Twitter account information, such as Verify Credentials and Rate Limit Status
        /// </summary>
        public TwitterQueryable<Account> Account
        {
            get
            {
                return new TwitterQueryable<Account>(this);
            }
        }

        /// <summary>
        /// Enables access to Twitter account activity information, such as listing webhooks and showing subscriptions.
        /// </summary>
        public TwitterQueryable<AccountActivity> AccountActivity
        {
            get
            {
                return new TwitterQueryable<AccountActivity>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter blocking information, such as Exists, Blocks, and IDs
        /// </summary>
        public TwitterQueryable<Blocks> Blocks
        {
            get
            {
                return new TwitterQueryable<Blocks>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Control Streams, which manage and query Site Streams
        /// </summary>
        public TwitterQueryable<ControlStream> ControlStream
        {
            get
            {
                return new TwitterQueryable<ControlStream>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Direct Messages
        /// </summary>
        public TwitterQueryable<DirectMessage> DirectMessage
        {
            get
            {
                return new TwitterQueryable<DirectMessage>(this);
            }
        }

        /// <summary>
        /// enables access to Direct Message Events, supporting Twitter chatbots
        /// </summary>
        public TwitterQueryable<DirectMessageEvents> DirectMessageEvents
        {
            get
            {
                return new TwitterQueryable<DirectMessageEvents>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Favorites
        /// </summary>
        public TwitterQueryable<Favorites> Favorites
        {
            get
            {
                return new TwitterQueryable<Favorites>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Friendship info
        /// </summary>
        public TwitterQueryable<Friendship> Friendship
        {
            get
            {
                return new TwitterQueryable<Friendship>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Geo info
        /// </summary>
        public TwitterQueryable<Geo> Geo
        {
            get
            {
                return new TwitterQueryable<Geo>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Help info
        /// </summary>
        public TwitterQueryable<Help> Help
        {
            get
            {
                return new TwitterQueryable<Help>(this);
            }
        }

        /// <summary>
        /// Enables access to media commands, like STATUS
        /// </summary>
        public TwitterQueryable<Media> Media
        {
            get
            {
                return new TwitterQueryable<Media>(this);
            }
        }

        /// <summary>
        /// Enables access to muted users
        /// </summary>
        public TwitterQueryable<Mute> Mute
        {
            get
            {
                return new TwitterQueryable<Mute>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter List info
        /// </summary>
        public TwitterQueryable<List> List
        {
            get
            {
                return new TwitterQueryable<List>(this);
            }
        }

        /// <summary>
        /// enables access to Raw Query Extensibility
        /// </summary>
        public TwitterQueryable<Raw> RawQuery
        {
            get
            {
                return new TwitterQueryable<Raw>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Saved Searches
        /// </summary>
        public TwitterQueryable<SavedSearch> SavedSearch
        {
            get
            {
                return new TwitterQueryable<SavedSearch>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Search to query tweets
        /// </summary>
        public TwitterQueryable<Search> Search
        {
            get
            {
                return new TwitterQueryable<Search>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Status messages
        /// </summary>
        public TwitterQueryable<Status> Status
        {
            get
            {
                return new TwitterQueryable<Status>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Status messages, such as Friends and Public
        /// </summary>
        public TwitterQueryable<Streaming> Streaming
        {
            get
            {
                return new TwitterQueryable<Streaming>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Trends, such as Trend, Current, Daily, and Weekly
        /// </summary>
        public TwitterQueryable<Trend> Trends
        {
            get
            {
                return new TwitterQueryable<Trend>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter User messages, such as Friends and Followers
        /// </summary>
        public TwitterQueryable<User> User
        {
            get
            {
                return new TwitterQueryable<User>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Vine messages, such as Oembed
        /// </summary>
        public TwitterQueryable<Vine> Vine
        {
            get
            {
                return new TwitterQueryable<Vine>(this);
            }
        }

        /// <summary>
        /// enables access to Twitter Welcome messages
        /// </summary>
        public TwitterQueryable<WelcomeMessage> WelcomeMessage
        {
            get
            {
                return new TwitterQueryable<LinqToTwitter.WelcomeMessage>(this);
            }
        }
    }
}
