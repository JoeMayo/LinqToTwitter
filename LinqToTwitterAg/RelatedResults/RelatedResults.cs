using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Information on related results - largely undocumented when first released, i.e. no indication of what Score and Kind mean.
    /// </summary>
    public class RelatedResults
    {
        public RelatedResults() { }
        public RelatedResults(JsonData resultsJson)
        {
            if (resultsJson == null) return;

            ResultAnnotations = new Annotation(resultsJson.GetValue<JsonData>("annotations"));
            Score = resultsJson.GetValue<double>("score");
            Kind = resultsJson.GetValue<string>("kind");
            JsonData value = resultsJson.GetValue<JsonData>("value");
            ValueAnnotations = new Annotation(value.GetValue<JsonData>("annotations"));
            Retweeted = value.GetValue<bool>("retweeted");
            InReplyToScreenName = value.GetValue<string>("in_reply_to_screen_name");
            var contributors = value.GetValue<JsonData>("contributors");
            Contributors =
                contributors == null ?
                    new List<Contributor>() :
                    (from JsonData contributor in contributors
                     select new Contributor(contributor))
                    .ToList();
            Coordinates = new Coordinate(value.GetValue<JsonData>("coordinates"));
            Place = new Place(value.GetValue<JsonData>("place"));
            User = new User(value.GetValue<JsonData>("user"));
            RetweetCount = value.GetValue<int>("retweet_count");
            IDString = value.GetValue<string>("id_str");
            InReplyToUserID = value.GetValue<ulong>("in_reply_to_user_id");
            Favorited = value.GetValue<bool>("favorited");
            InReplyToStatusIDString = value.GetValue<string>("in_reply_to_status_id_str");
            InReplyToStatusID = value.GetValue<ulong>("in_reply_to_status_id");
            Source = value.GetValue<string>("source");
            CreatedAt = value.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
            InReplyToUserIDString = value.GetValue<string>("in_reply_to_user_id_str");
            Truncated = value.GetValue<bool>("truncated");
            Geo = new Geo(value.GetValue<JsonData>("geo"));
            Text = value.GetValue<string>("text");
        }

        /// <summary>
        /// Type of result
        /// </summary>
        public RelatedResultsType Type { get; set; }

        /// <summary>
        /// Tweet ID to get results for
        /// </summary>
        public ulong StatusID { get; set; }

        /// <summary>
        /// Annotations associated with Result Entry
        /// </summary>
        public Annotation ResultAnnotations { get; set; }

        /// <summary>
        /// Annotations associated with Tweet
        /// </summary>
        public Annotation ValueAnnotations { get; set; }

        /// <summary>
        /// Tweet Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Is tweet truncated?
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// Place info for where Tweet originated
        /// </summary>
        public Place Place { get; set; }

        /// <summary>
        /// Latitude & Longitude on Tweet
        /// </summary>
        public Coordinate Coordinates { get; set; }

        /// <summary>
        /// Is tweet marked as a favorite
        /// </summary>
        public bool Favorited { get; set; }

        /// <summary>
        /// String representation of Tweet ID
        /// </summary>
        public string IDString { get; set; }

        /// <summary>
        /// Has tweet been retweeted?
        /// </summary>
        public bool Retweeted { get; set; }

        /// <summary>
        /// Number of retweets, Max is 100
        /// </summary>
        public int RetweetCount { get; set; }

        /// <summary>
        /// What software or application produced tweet
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// When was this tweeted
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Geographic location of result
        /// </summary>
        public Geo Geo { get; set; }

        /// <summary>
        /// List of contributors to tweet
        /// </summary>
        public List<Contributor> Contributors { get; set; }

        /// <summary>
        /// Screen Name of replier
        /// </summary>
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// ID of replier
        /// </summary>
        public string InReplyToStatusIDString { get; set; }

        /// <summary>
        /// ID of tweet replying to StatusID tweet
        /// </summary>
        public ulong InReplyToStatusID { get; set; }

        /// <summary>
        /// User ID, in string format, of person replying to tweet
        /// </summary>
        public string InReplyToUserIDString { get; set; }

        /// <summary>
        /// User ID of person replying to tweet
        /// </summary>
        public ulong InReplyToUserID { get; set; }

        /// <summary>
        /// Info on user who tweeted
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Kind
        /// </summary>
        public string Kind { get; set; }
    }
}
