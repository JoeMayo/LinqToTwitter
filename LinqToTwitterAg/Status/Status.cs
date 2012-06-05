/***********************************************************
 * Credits:
 * 
 * Written by: Joe Mayo, 8/26/08
 * *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// returned information from Twitter Status queries
    /// </summary>
    public class Status
    {
        public Status() {}
        public Status(JsonData status)
        {
            if (status == null) return;

            Retweeted = status.GetValue<bool>("retweeted");
            Source = status.GetValue<string>("source");
            InReplyToScreenName = status.GetValue<string>("in_reply_to_screen_name");
            PossiblySensitive = status.GetValue<bool>("possibly_sensitive");
            RetweetedStatus = new Status(status.GetValue<JsonData>("retweeted_status"));
            var contributors = status.GetValue<JsonData>("contributors");
            if (contributors != null)
                Contributors =
                    (from JsonData contributor in contributors
                     select (ulong)contributor)
                    .ToList();
            else
                Contributors = new List<ulong>();
            var coords = status.GetValue<JsonData>("coordinates");
            if (coords != null)
            {
                Coordinates = new Coordinate(coords.GetValue<JsonData>("coordinates"));
            }
            else
            {
                Coordinates = new Coordinate();
            }
            Place = new Place(status.GetValue<JsonData>("place"));
            User = new User(status.GetValue<JsonData>("user"));
            RetweetCount = status.GetValue<int>("retweet_count");
            StatusID = status.GetValue<string>("id_str");
            Favorited = status.GetValue<bool>("favorited");
            InReplyToStatusID = status.GetValue<string>("in_reply_to_status_id_str");
            Source = status.GetValue<string>("source");
            CreatedAt = status.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
            InReplyToUserID = status.GetValue<string>("in_reply_to_user_id_str");
            Truncated = status.GetValue<bool>("truncated");
            Geo = new Geo(status.GetValue<JsonData>("geo"));
            Text = status.GetValue<string>("text");
            Annotation = new Annotation(status.GetValue<JsonData>("annotation"));
            Entities = new Entities(status.GetValue<JsonData>("entities"));
        }

        /// <summary>
        /// Supports XML serialization
        /// </summary>
        [XmlIgnore]
        StatusType type;

        /// <summary>
        /// type of status request, i.e. Friends or Public
        /// </summary>
        [XmlIgnore]
        public StatusType Type
        {
            get { return type; }
            set { type = value; }
        }

        [XmlAttribute(AttributeName = "Type")]
        StatusType StatusTypeXml
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// TweetID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// User ID to disambiguate when ID is same as screen name
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Screen Name to disambiguate when ID is same as UserD
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// filter results to after this status id
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// max ID to retrieve
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// only return this many results
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// page of results to return
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// By default, user timeline doesn't include retweets,
        /// but you can set this to true to includes retweets
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including retweets will return them regardless of the value provided.")]
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Don't include replies in results
        /// </summary>
        public bool ExcludeReplies { get; set; }

        /// <summary>
        /// Add entities to tweets
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including entities will return them regardless of the value provided.")]
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// when was the tweet created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of this status
        /// </summary>
        public string StatusID { get; set; }

        /// <summary>
        /// Tweet Text (140)characters
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// where did the tweet come from
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// has the tweet been truncated
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// id of tweet being replied to, if it is a reply
        /// </summary>
        public string InReplyToStatusID { get; set; }

        /// <summary>
        /// id of user being replied to, if it is a reply
        /// </summary>
        public string InReplyToUserID { get; set; }

        /// <summary>
        /// is listed as a favorite
        /// </summary>
        public bool Favorited { get; set; }

        /// <summary>
        /// screen name of user being replied to, if it is a reply
        /// </summary>
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// information about user posting tweet (except in user tweets)
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Retweet details
        /// </summary>
        [Obsolete("Deprecated: Use RetweetedStatus instead.", true)]
        public Retweet Retweet { get; set; }

        /// <summary>
        /// IDs of users who have contributed
        /// </summary>
        public List<ulong> Contributors { get; set; }

        /// <summary>
        /// Geographic information on tweet location
        /// </summary>
        [Obsolete("Soon to be deprecated. Use Coordinates instead.")]
        public Geo Geo { get; set; }

        /// <summary>
        /// Coordinates of where tweet occurred
        /// </summary>
        public Coordinate Coordinates { get; set; }

        /// <summary>
        /// Place where status was created
        /// </summary>
        public Place Place { get; set; }

        /// <summary>
        /// Meta-data applied to tweet
        /// </summary>
        public Annotation Annotation { get; set; }

        /// <summary>
        /// Entities connected to the status
        /// </summary>
        public Entities Entities { get; set; }

        /// <summary>
        /// Removes all user info, except for ID
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Include more contributor info, beyond ID
        /// </summary>
        public bool IncludeContributorDetails { get; set; }

        /// <summary>
        /// Number of times retweeted
        /// </summary>
        public int RetweetCount { get; set; }

        /// <summary>
        /// Has tweet been retweeted
        /// </summary>
        public bool Retweeted { get; set; }

        /// <summary>
        /// Is tweet possibly sensitive (can be set via TweetWithMedia)
        /// </summary>
        public bool PossiblySensitive { get; set; }

        /// <summary>
        /// Retweeted status is status is a retweet
        /// </summary>
        public Status RetweetedStatus { get; set; }
    }
}
