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
        /// <summary>
        /// Shreds an XML element into a Status object
        /// </summary>
        /// <param name="status">XML element with info</param>
        /// <returns>Newly populated status object</returns>
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

            List<string> contributorIDs = null;

            if (status.Element("contributors") != null)
            {
                contributorIDs =
                    (from id in status.Element("contributors").Elements("user_id")
                     select id.Value)
                     .ToList(); 
            }

            XNamespace geoRss = "http://www.georss.org/georss";

            var geoStr =
               status.Element("geo") != null &&
               status.Element("geo").Element(geoRss + "point") != null ?
                   status.Element("geo").Element(geoRss + "point").Value :
                   string.Empty;

            Geo geo = new Geo();
            if (!string.IsNullOrEmpty(geoStr))
            {
                var coordArr = geoStr.Split(' ');

                double tempLatitude = 0;
                double tempLongitide = 0;

                if (double.TryParse(coordArr[Coordinate.LatitudePos], out tempLatitude) &&
                    double.TryParse(coordArr[Coordinate.LongitudePos], out tempLongitide))
                {
                    geo =
                        new Geo
                        {
                            Latitude = tempLatitude,
                            Longitude = tempLongitide
                        };
                }
            }
            
            var coordStr =
                status.Element("coordinates") != null &&
                status.Element("coordinates").Element(geoRss + "point") != null ?
                    status.Element("coordinates").Element(geoRss + "point").Value :
                    string.Empty;

            Coordinate coord = new Coordinate();
            if (!string.IsNullOrEmpty(coordStr))
            {
                var coordArr = coordStr.Split(' ');

                double tempLatitude = 0;
                double tempLongitide = 0;

                if (double.TryParse(coordArr[Coordinate.LatitudePos], out tempLatitude) &&
                    double.TryParse(coordArr[Coordinate.LongitudePos], out tempLongitide))
                {
                    coord = 
                        new Coordinate
                        {
                            Latitude = tempLatitude,
                            Longitude = tempLongitide
                        }; 
                }
            }

            var place = new Place().CreatePlace(status.Element("place"));

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
                ContributorIDs = contributorIDs,
                Geo = geo,
                Coordinates = coord,
                Place = place,
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
        /// By default, user timeline doesn't include retweets,
        /// but you can set this to true to includes retweets
        /// </summary>
        public bool IncludeRetweets { get; set; }

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

        /// <summary>
        /// Contains ID of users who have contributed
        /// </summary>
        public List<string> ContributorIDs { get; set; }

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
    }
}
