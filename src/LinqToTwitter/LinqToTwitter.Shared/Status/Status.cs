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
    [XmlType(Namespace = "LinqToTwitter")]
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
            IsQuotedStatus = status.GetValue<bool>("is_quote_status");
            QuotedStatusID = status.GetValue<ulong>("quoted_status_id");
            QuotedStatus = new Status(status.GetValue<JsonData>("quoted_status"));
            JsonData contributors = status.GetValue<JsonData>("contributors");
            Contributors =
                contributors == null ?
                    new List<Contributor>() :
                    (from JsonData contributor in contributors
                     select new Contributor(contributor))
                    .ToList();
            JsonData coords = status.GetValue<JsonData>("coordinates");
            Coordinates = coords != null ?
                new Coordinate(coords.GetValue<JsonData>("coordinates")) :
                new Coordinate();
            Place = new Place(status.GetValue<JsonData>("place"));
            RetweetCount = status.GetValue<int>("retweet_count");
            StatusID = status.GetValue<ulong>("id");
            FavoriteCount = status.GetValue<int?>("favorite_count");
            Favorited = status.GetValue<bool>("favorited");
            InReplyToStatusID = status.GetValue<ulong>("in_reply_to_status_id");
            CreatedAt = status.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
            InReplyToUserID = status.GetValue<ulong>("in_reply_to_user_id");
            Truncated = status.GetValue<bool>("truncated");
            JsonData displayTextIndices = status.GetValue<JsonData>("display_text_range");
            DisplayTextRange = new List<int> { (int) displayTextIndices[0], (int) displayTextIndices[1] };
            TweetMode tweetMode;
            Enum.TryParse(value: status.GetValue<string>("tweet_mode"), ignoreCase: true, result: out tweetMode);
            TweetMode = tweetMode;
            Text = status.GetValue<string>("text");
            FullText = status.GetValue<string>("full_text");
            ExtendedTweet = new Status(status.GetValue<JsonData>("extended_tweet"));
            Annotation = new Annotation(status.GetValue<JsonData>("annotation"));
            Entities = new Entities(status.GetValue<JsonData>("entities"));
            ExtendedEntities = new Entities(status.GetValue<JsonData>("extended_entities"));
            JsonData currentUserRetweet = status.GetValue<JsonData>("current_user_retweet");
            if (currentUserRetweet != null)
                CurrentUserRetweet = currentUserRetweet.GetValue<ulong>("id");
            JsonData scopes = status.GetValue<JsonData>("scopes");
            Scopes =
                scopes == null ? new Dictionary<string, string>() :
                (from key in (scopes as IDictionary<string, JsonData>).Keys as List<string>
                 select new
                 {
                     Key = key,
                     Value = scopes[key].ToString()
                 })
                .ToDictionary(
                    key => key.Key,
                    val => val.Value);
            WithheldCopyright = status.GetValue<bool>("withheld_copyright");
            JsonData withheldCountries = status.GetValue<JsonData>("withheld_in_countries");
            WithheldInCountries =
                withheldCountries == null ? new List<string>() :
                (from JsonData country in status.GetValue<JsonData>("withheld_in_countries")
                 select country.ToString())
                .ToList();
            WithheldScope = status.GetValue<string>("withheld_scope");
            MetaData = new StatusMetaData(status.GetValue<JsonData>("metadata"));
            Lang = status.GetValue<string>("lang");
            FilterLevel filterLevel;
            Enum.TryParse(value: status.GetValue<string>("filter_level"), ignoreCase: true, result: out filterLevel);
            FilterLevel = filterLevel;
            User = new User(status.GetValue<JsonData>("user"));
            Users = new List<ulong>();
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
        internal StatusType StatusTypeXml
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// TweetID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// ID of User
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// User Screen Name
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
        /// Next page of data to return
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// By default, user timeline doesn't include retweets,
        /// but you can set this to true to includes retweets
        /// </summary>
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Don't include replies in results
        /// </summary>
        public bool ExcludeReplies { get; set; }

        /// <summary>
        /// Add entities to tweets (default: true)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Add entities to user (default: true)
        /// </summary>
        public bool IncludeUserEntities { get; set; }

        /// <summary>
        /// Populates CurrentUserRetweet in response if set to true
        /// </summary>
        public bool IncludeMyRetweet { get; set; }

        /// <summary>
        /// Url of tweet to embed
        /// </summary>
        public string OEmbedUrl { get; set; }

        /// <summary>
        /// Max number of pixels for width
        /// </summary>
        public int OEmbedMaxWidth { get; set; }

        /// <summary>
        /// Don't initially expand image
        /// </summary>
        public bool OEmbedHideMedia { get; set; }

        /// <summary>
        /// Show original message for replies
        /// </summary>
        public bool OEmbedHideThread { get; set; }

        /// <summary>
        /// Don't include widgets.js script
        /// </summary>
        public bool OEmbedOmitScript { get; set; }

        /// <summary>
        /// Image alignment: Left, Right, Center, or None
        /// </summary>
        public EmbeddedStatusAlignment OEmbedAlign { get; set; }

        /// <summary>
        /// Suggested accounts for the viewer to follow
        /// </summary>
        public string OEmbedRelated { get; set; }

        /// <summary>
        /// Language code for rendered tweet
        /// </summary>
        public string OEmbedLanguage { get; set; }

        /// <summary>
        /// when was the tweet created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of this status
        /// </summary>
        public ulong StatusID { get; set; }

        /// <summary>
        /// Tweet Text (140)characters
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// When a tweet is an extended tweet in extended mode, 
        /// Text will be null and FullText will contain the tweet text.
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// Extended tweet with entities in extended mode.
        /// </summary>
        public Status ExtendedTweet { get; set; }

        /// <summary>
        /// where did the tweet come from
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Has the tweet been truncated? True means that this is compatibiltiy mode tweet.
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// Inclusive start and exclusive end of displayable tweet content.
        /// </summary>
        public List<int> DisplayTextRange { get; set; }

        /// <summary>
        /// Tweets can be compatibility or extended mode. Extended is the 
        /// new mode that allows you to put more characters in a tweet.
        /// </summary>
        public TweetMode TweetMode { get; set; }

        /// <summary>
        /// id of tweet being replied to, if it is a reply
        /// </summary>
        public ulong InReplyToStatusID { get; set; }

        /// <summary>
        /// id of user being replied to, if it is a reply
        /// </summary>
        public ulong InReplyToUserID { get; set; }

        /// <summary>
        /// Number of times this tweet has been favorited
        /// </summary>
        public int? FavoriteCount { get; set; }

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
        /// Users who retweeted a tweet (used in StatusType.RetweetedBy queries)
        /// </summary>
        public List<ulong> Users { get; set; }

        /// <summary>
        /// users who have contributed
        /// </summary>
        public List<Contributor> Contributors { get; set; }

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
        /// Additional entities connected to the status
        /// </summary>
        public Entities ExtendedEntities { get; set; }

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

        /// <summary>
        /// ID of source status of retweet if IncludeMyRetweet is true.
        /// Set to 0 if IncludeMyRetweet is false or tweet not retweeted
        /// by authenticating user.
        /// </summary>
        public ulong CurrentUserRetweet { get; set; }

        /// <summary>
        /// Is this status quoting another tweet
        /// </summary>
        public bool IsQuotedStatus { get; set; }

        /// ID of the quoted status
        /// </summary>
        public ulong QuotedStatusID { get; set; }

        /// <summary>
        /// Complete Status object representing the quoted status
        /// </summary>
        public Status QuotedStatus { get; set; }

        /// <summary>
        /// Set of key/value pairs to support promoted tweets
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> Scopes { get; set; }

        /// <summary>
        /// Indicates that you shouldn't display because there
        /// is currently a DMCA complaint against the tweet.
        /// </summary>
        public bool WithheldCopyright { get; set; }

        /// <summary>
        /// Don't display tweet in countries in this list
        /// </summary>
        public List<string> WithheldInCountries { get; set; }

        /// <summary>
        /// Part of the tweet that should not be displayed.
        /// </summary>
        public string WithheldScope { get; set; }

        /// <summary>
        /// Status meta-data returned from searches
        /// </summary>
        public StatusMetaData MetaData { get; set; }

        /// <summary>
        /// Twitter machine-detected prediction of language tweet is written in
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Indicate that a status lookup should return null objects for 
        /// tweets that the authorizing user doesn't have access to. 
        /// (e.g. tweet is from a protected account or doesn't exist)
        /// </summary>
        public bool Map { get; set; }

        /// <summary>
        /// Comma-separated list of tweet IDs passed to Lookup.
        /// </summary>
        public string TweetIDs { get; set; }

        /// <summary>
        /// Twitter's evaluation of tweet quality
        /// </summary>
        public FilterLevel FilterLevel { get; set; }

        /// <summary>
        /// Populated with OEmbed response for StatusType.OEmbed queries
        /// </summary>
        public EmbeddedStatus EmbeddedStatus { get; set; }

        /// <summary>
        /// Manage paging through a list (e.g. IDs from Users collection)
        /// </summary>
        public Cursors CursorMovement { get; set; }

        /// <summary>
        /// This helps process media uploads via StatusRequestProcessor.ProcessActionResult
        /// </summary>
        internal Media Media { get; set; }
    }
}
