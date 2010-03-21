/***********************************************************
 * Credits:
 * 
 * Written by: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// returned information from Twitter Status queries
    /// </summary>
    [Serializable]
    public class Status
    {
        public Status CreateStatus(XElement status)
        {
            if (status == null)
            {
                return null;
            }

            var dateParts =
                status.Element("created_at").Value.Split(' ');

            var createdAtDate =
                dateParts.Count() > 1 ?
                DateTime.Parse(
                    string.Format("{0} {1} {2} {3} GMT",
                    dateParts[1],
                    dateParts[2],
                    dateParts[5],
                    dateParts[3]),
                    CultureInfo.InvariantCulture) :
                DateTime.MinValue;

            var user = status.Element("user");

            var retweet = status.Element("retweeted_status");

            var rtDateParts =
                retweet == null ?
                    null :
                    retweet.Element("created_at").Value.Split(' ');

            var retweetedAtDate =
                retweet == null ?
                    DateTime.MinValue :
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT",
                        rtDateParts[1],
                        rtDateParts[2],
                        rtDateParts[5],
                        rtDateParts[3]),
                        CultureInfo.InvariantCulture);

            var usr = new User();

            var newStatus = new Status
            {
                CreatedAt = createdAtDate,
                Favorited =
                 bool.Parse(
                     string.IsNullOrEmpty(status.Element("favorited").Value) ?
                     "true" :
                     status.Element("favorited").Value),
                StatusID = status.Element("id").Value,
                InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                Source = status.Element("source").Value,
                Text = status.Element("text").Value,
                Truncated = bool.Parse(status.Element("truncated").Value),
                InReplyToScreenName =
                     status.Element("in_reply_to_screen_name") == null ?
                         string.Empty :
                         status.Element("in_reply_to_screen_name").Value,
                User = usr.CreateUser(user),
                Retweet =
                    retweet == null ?
                        null :
                        new Retweet
                        {
                            ID = retweet.Element("id").Value,
                            CreatedAt = retweetedAtDate,
                            Favorited =
                                bool.Parse(
                                    string.IsNullOrEmpty(retweet.Element("favorited").Value) ?
                                    "true" :
                                    retweet.Element("favorited").Value),
                            InReplyToScreenName = retweet.Element("in_reply_to_screen_name").Value,
                            InReplyToStatusID = retweet.Element("in_reply_to_status_id").Value,
                            InReplyToUserID = retweet.Element("in_reply_to_user_id").Value,
                            Source = retweet.Element("source").Value,
                            Text = retweet.Element("text").Value,
                            Truncated =
                                bool.Parse(
                                    string.IsNullOrEmpty(retweet.Element("truncated").Value) ?
                                    "true" :
                                    retweet.Element("truncated").Value),
                            RetweetingUser = usr.CreateUser(retweet.Element("user"))
                        }
            };

            return newStatus;
        }

        /// <summary>
        /// type of status request, i.e. Friends or Public
        /// </summary>
        public StatusType Type { get; set; }

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
        public Retweet Retweet { get; set; }
    }
}
