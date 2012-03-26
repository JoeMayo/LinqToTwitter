/***********************************************************
 * Credits:
 * 
 * Written by: Joe Mayo, 8/26/08
 * *********************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
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
        internal Status(JsonData status)
        {
            if (status == null) return;

            CreatedAt = status.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
            Favorited = status.GetValue<bool>("favorited");
            StatusID = status.GetValue<ulong>("id").ToString(CultureInfo.InvariantCulture);
            InReplyToStatusID = status.GetValue<ulong>("in_reply_to_status_id").ToString(CultureInfo.InvariantCulture);
            InReplyToUserID = status.GetValue<ulong>("in_reply_to_user_id").ToString(CultureInfo.InvariantCulture);
            Source = status.GetValue<string>("source");
            Text = status.GetValue<string>("text");
            Truncated = status.GetValue<bool>("truncated");
            InReplyToScreenName = status.GetValue<string>("in_reply_to_screen_name");
            Contributors = new List<Contributor>();
            Coordinates = new Coordinate();
            Place = new Place(status.GetValue<JsonData>("place"));
            User = new User(status.GetValue<JsonData>("user"));
        }


        /// <summary>
        /// Shreds an XML element into a Status object
        /// </summary>
        /// <param name="status">XML element with info</param>
        /// <returns>Newly populated status object</returns>
        public static Status CreateStatus(XElement status)
        {
            if (status == null)
            {
                return null;
            }

            string dateString = null;
            var createdAtElement = status.Element("created_at");
            if (createdAtElement != null) dateString = createdAtElement.Value;

            var createdAtDate = String.IsNullOrEmpty(dateString) 
                                ? DateTime.MinValue
                                : DateTime.ParseExact(
                                        dateString,
                                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            var user = status.Element("user");
            var retweet = status.Element("retweeted_status");

            var retweetCount =
                status.Element("retweet_count") == null || 
                status.Element("retweet_count").Value == string.Empty ?
                    0 :
                    int.Parse(status.Element("retweet_count").Value.TrimEnd('+'));

            var retweeted =
                status.Element("retweeted") == null || status.Element("retweeted").Value == string.Empty ?
                    false :
                    bool.Parse(status.Element("retweeted").Value);

            var retweetDate = retweet == null
                              ? null
                              : retweet.Element("created_at").Value;

            var retweetedAtDate = String.IsNullOrEmpty(retweetDate)
                                ? DateTime.MinValue
                                : DateTime.ParseExact(
                                        retweetDate,
                                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);


            List<Contributor> contributors = null;

            XElement contributorElement = status.Element("contributors");

            if (contributorElement != null)
            {
                if (contributorElement.Elements("user").Count() > 0)
                {
                    contributors =
                        (from contr in contributorElement.Elements("user")
                         select new Contributor
                         {
                             ID = contr.Element("id").Value,
                             ScreenName = contr.Element("screen_name").Value
                         })
                        .ToList();
                }
                else
                {
                    contributors =
                            (from id in contributorElement.Elements("user_id")
                             select new Contributor
                             {
                                 ID = id.Value,
                                 ScreenName = string.Empty
                             })
                            .ToList();  
                }
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

            var place = Place.CreatePlace(status.Element("place"));
            var annotation = Annotation.CreateAnnotation(status.Element("annotation"));
            var entities = Entities.CreateEntities(status.Element("entities"));

            var newStatus = new Status
            {
                CreatedAt = createdAtDate,
                Favorited = status.GetBool("favorited", false),
                StatusID = status.GetString("id"),
                InReplyToStatusID = status.GetString("in_reply_to_status_id"),
                InReplyToUserID = status.GetString("in_reply_to_user_id"),
                Source = status.GetString("source"),
                Text = status.GetString("text"),
                Truncated = status.GetBool("truncated"),
                InReplyToScreenName = status.GetString("in_reply_to_screen_name"),
                Contributors = contributors,
                Geo = geo,
                Coordinates = coord,
                Place = place,
                Annotation = annotation,
                User = User.CreateUser(user),
                Entities = entities,
                Retweeted = retweeted,
                RetweetCount = retweetCount,
                Retweet =
                    retweet == null ?
                        null :
                        new Retweet
                        {
                            ID = retweet.GetString("id"),
                            CreatedAt = retweetedAtDate,
                            Favorited = retweet.GetBool("favorited"),
                            InReplyToScreenName = retweet.GetString("in_reply_to_screen_name"),
                            InReplyToStatusID = retweet.GetString("in_reply_to_status_id"),
                            InReplyToUserID = retweet.GetString("in_reply_to_user_id"),
                            Source = retweet.GetString("source"),
                            Text = retweet.GetString("text"),
                            Retweeted = retweet.GetBool("retweeted"),
                            RetweetCount = 
                                //retweet.GetInt("retweet_count"),
                                retweet.Element("retweet_count") == null ||
                                retweet.Element("retweet_count").Value == string.Empty ?
                                    0 :
                                    int.Parse(retweet.Element("retweet_count").Value.TrimEnd('+')),
                            Truncated = retweet.GetBool("truncated", true),
                            RetweetedUser = User.CreateUser(retweet.Element("user"))
                        }
            };

            return newStatus;
        }

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
        public Retweet Retweet { get; set; }

        /// <summary>
        /// Contains info on users who have contributed
        /// </summary>
        public List<Contributor> Contributors { get; set; }

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
        public object RetweetCount { get; set; }

        /// <summary>
        /// Has tweet been retweeted
        /// </summary>
        public object Retweeted { get; set; }
    }
}
