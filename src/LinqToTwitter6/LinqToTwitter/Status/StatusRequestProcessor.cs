using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Status requests
    /// </summary>
    public class StatusRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// type of status request, i.e. Show or User
        /// </summary>
        public StatusType Type { get; set; }

        /// <summary>
        /// TweetID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// User ID to disambiguate when ID is same as screen name
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// Screen Name to disambiguate when ID is same as UserD
        /// </summary>
        public string? ScreenName { get; set; }

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
        /// Retweets are optional and you must set this to true
        /// before they will be included in the user timeline
        /// </summary>
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Don't include replies in responses
        /// </summary>
        public bool ExcludeReplies { get; set; }

        /// <summary>
        /// Include entities in tweets (default: true)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Include entities in users (default: true)
        /// </summary>
        public bool IncludeUserEntities { get; set; }

        /// <summary>
        /// Remove all user info, except for User ID
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Enhances contributor info, beyond the default ID
        /// </summary>
        public bool IncludeContributorDetails { get; set; }

        /// <summary>
        /// Populates CurrentUserRetweet in response if set to true
        /// </summary>
        public bool IncludeMyRetweet { get; set; }

        /// <summary>
        /// Includes Alt Text, if available
        /// </summary>
        public bool IncludeAltText { get; set; }

        /// <summary>
        /// Indicate that a status lookup should return null objects for 
        /// tweets that the authorizing user doesn't have access to. 
        /// (e.g. tweet is from a protected account or doesn't exist)
        /// </summary>
        public bool Map { get; set; }

        /// <summary>
        /// Url of tweet to embed
        /// </summary>
        public string? OEmbedUrl { get; set; }

        /// <summary>
        /// Max number of pixels for width
        /// </summary>
        public int OEmbedMaxWidth { get; set; }

        /// <summary>
        /// Don't initially expand media
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
        public string? OEmbedRelated { get; set; }

        /// <summary>
        /// Language code for rendered tweet
        /// </summary>
        public string? OEmbedLanguage { get; set; }

        /// <summary>
        /// Comma-separated list of tweet IDs passed to Lookup.
        /// </summary>
        public string? TweetIDs { get; set; }

        /// <summary>
        /// Tweets can be compatibility or extended mode. Extended is the 
        /// new mode that allows you to put more characters in a tweet.
        /// </summary>
        public TweetMode TweetMode { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Status>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "UserID",
                       "ScreenName",
                       "SinceID",
                       "MaxID",
                       "Count",
                       "Cursor",
                       "IncludeRetweets",
                       "ExcludeReplies",
                       "IncludeEntities",
                       "IncludeUserEntities",
                       "TrimUser",
                       "IncludeContributorDetails",
                       "IncludeMyRetweet",
                       nameof(IncludeAltText),
                       "Map",
                       "OEmbedUrl",
                       "OEmbedMaxWidth",
                       "OEmbedHideMedia",
                       "OEmbedHideThread",
                       "OEmbedOmitScript",
                       "OEmbedAlign",
                       "OEmbedRelated",
                       "OEmbedLanguage",
                       "TweetIDs",
                       nameof(TweetMode)
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<StatusType>(parameters["Type"]);

            switch (Type)
            {
                case StatusType.Home:
                    return BuildHomeUrl(parameters);
                case StatusType.Mentions:
                    return BuildMentionsUrl(parameters);
                case StatusType.Oembed:
                    return BuildOembedUrl(parameters);
                case StatusType.RetweetsOfMe:
                    return BuildRetweetsOfMeUrl(parameters);
                case StatusType.Retweets:
                    return BuildRetweets(parameters);
                case StatusType.User:
                    return BuildUserUrl(parameters);
                case StatusType.Retweeters:
                    return BuildRetweetersUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// appends parameters that are common to both friend and user queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ID"))
            {
                ID = ulong.Parse(parameters["ID"]);
                urlParams.Add(new QueryParameter("id", parameters["ID"]));
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", parameters["SinceID"]));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", parameters["MaxID"]));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("IncludeRetweets"))
            {
                IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);
                urlParams.Add(new QueryParameter("include_rts", parameters["IncludeRetweets"].ToLower()));
            }

            if (parameters.ContainsKey("ExcludeReplies"))
            {
                ExcludeReplies = bool.Parse(parameters["ExcludeReplies"]);
                urlParams.Add(new QueryParameter("exclude_replies", parameters["ExcludeReplies"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeMyRetweet"))
            {
                IncludeMyRetweet = bool.Parse(parameters["IncludeMyRetweet"]);
                urlParams.Add(new QueryParameter("include_my_retweet", parameters["IncludeMyRetweet"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeUserEntities"))
            {
                IncludeUserEntities = bool.Parse(parameters["IncludeUserEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", parameters["IncludeUserEntities"].ToLower()));
            }

            if (parameters.ContainsKey("TrimUser"))
            {
                TrimUser = bool.Parse(parameters["TrimUser"]);
                urlParams.Add(new QueryParameter("trim_user", parameters["TrimUser"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeContributorDetails"))
            {
                IncludeContributorDetails = bool.Parse(parameters["IncludeContributorDetails"]);
                urlParams.Add(new QueryParameter("contributor_details", parameters["IncludeContributorDetails"].ToLower()));
            }

            if (parameters.ContainsKey(nameof(TweetMode)))
            {
                TweetMode = RequestProcessorHelper.ParseEnum<TweetMode>(parameters[nameof(TweetMode)]);
                urlParams.Add(new QueryParameter("tweet_mode", TweetMode.ToString().ToLower()));
            }

            if (parameters.ContainsKey(nameof(IncludeAltText)))
            {
                IncludeAltText = bool.Parse(parameters[nameof(IncludeAltText)]);
                urlParams.Add(new QueryParameter("include_ext_alt_text", parameters[nameof(IncludeAltText)].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// construct a base home url
        /// </summary>
        /// <returns>base url + home segment</returns>
        Request BuildHomeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/home_timeline.json");
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">parameters to build url query with</param>
        /// <returns>base url + mentions segment</returns>
        Request BuildMentionsUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/mentions_timeline.json");
        }

        /// <summary>
        /// construct an oembed url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + url segment</returns>
        Request BuildOembedUrl(Dictionary<string, string> parameters)
        {
            var req = new Request("https://publish.twitter.com/oembed");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ID"))
            {
                ID = ulong.Parse(parameters["ID"]);
                urlParams.Add(new QueryParameter("id", parameters["ID"]));
            }

            if (parameters.ContainsKey("OEmbedUrl"))
            {
                OEmbedUrl = parameters["OEmbedUrl"];
                urlParams.Add(new QueryParameter("url", parameters["OEmbedUrl"]));
            }
            else
            {
                throw new ArgumentException($"{nameof(OEmbedUrl)} is required!", nameof(OEmbedUrl));
            }

            if (parameters.ContainsKey("OEmbedMaxWidth"))
            {
                OEmbedMaxWidth = int.Parse(parameters["OEmbedMaxWidth"]);
                urlParams.Add(new QueryParameter("maxwidth", parameters["OEmbedMaxWidth"]));
            }

            if (parameters.ContainsKey("OEmbedHideMedia"))
            {
                OEmbedHideMedia = bool.Parse(parameters["OEmbedHideMedia"]);
                urlParams.Add(new QueryParameter("hide_media", parameters["OEmbedHideMedia"].ToLower()));
            }

            if (parameters.ContainsKey("OEmbedHideThread"))
            {
                OEmbedHideThread = bool.Parse(parameters["OEmbedHideThread"]);
                urlParams.Add(new QueryParameter("hide_thread", parameters["OEmbedHideThread"].ToLower()));
            }

            if (parameters.ContainsKey("OEmbedOmitScript"))
            {
                OEmbedOmitScript = bool.Parse(parameters["OEmbedOmitScript"]);
                urlParams.Add(new QueryParameter("omit_script", parameters["OEmbedOmitScript"].ToLower()));
            }

            if (parameters.ContainsKey("OEmbedAlign"))
            {
                OEmbedAlign = RequestProcessorHelper.ParseEnum<EmbeddedStatusAlignment>(parameters["OEmbedAlign"]);
                urlParams.Add(new QueryParameter("align", OEmbedAlign.ToString().ToLower()));
            }

            if (parameters.ContainsKey("OEmbedRelated"))
            {
                OEmbedRelated = parameters["OEmbedRelated"];
                urlParams.Add(new QueryParameter("related", parameters["OEmbedRelated"].Replace(" ", "")));
            }

            if (parameters.ContainsKey("OEmbedLanguage"))
            {
                OEmbedLanguage = parameters["OEmbedLanguage"];
                urlParams.Add(new QueryParameter("lang", parameters["OEmbedLanguage"].ToLower()));
            }

            if (parameters.ContainsKey(nameof(TweetMode)))
            {
                TweetMode = RequestProcessorHelper.ParseEnum<TweetMode>(parameters[nameof(TweetMode)]);
                urlParams.Add(new QueryParameter("tweet_mode", TweetMode.ToString().ToLower()));
            }

            return req;
        }

        /// <summary>
        /// construct a base retweeted by user url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted by user segment</returns>
        Request BuildRetweets(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
                throw new ArgumentNullException("ID", "ID is required.");

            ID = ulong.Parse(parameters["ID"]);

            var req = new Request(BaseUrl + "statuses/retweets/" + ID + ".json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("TrimUser"))
            {
                TrimUser = bool.Parse(parameters["TrimUser"]);
                urlParams.Add(new QueryParameter("trim_user", parameters["TrimUser"].ToLower()));
            }

            return req;
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweets of me segment</returns>
        Request BuildRetweetsOfMeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweets_of_me.json");
        }

        Request BuildRetweetersUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
                throw new ArgumentException("ID is required.", "ID");

            var url = BaseUrl + "statuses/retweeters/ids.json";
            var req = new Request(url);
            var urlParams = req.RequestParameters;

            ID = ulong.Parse(parameters["ID"]);
            urlParams.Add(new QueryParameter("id", parameters["ID"]));

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);

                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey(nameof(TweetMode)))
            {
                TweetMode = RequestProcessorHelper.ParseEnum<TweetMode>(parameters[nameof(TweetMode)]);
                urlParams.Add(new QueryParameter("tweet_mode", TweetMode.ToString().ToLower()));
            }

            return req;
        }

        /// <summary>
        /// construct an url for the user timeline
        /// </summary>
        /// <returns>base url + user timeline segment</returns>
        Request BuildUserUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/user_timeline.json");
        }

        /// <summary>
        /// transforms Twitter response into List of Status
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of Status</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonElement statusJson = JsonDocument.Parse(responseJson).RootElement;

            List<Status> statusList;
            switch (Type)
            {
                case StatusType.Home:
                case StatusType.Mentions:
                case StatusType.RetweetsOfMe:
                case StatusType.Retweets:
                case StatusType.User:
                    statusList =
                        (from status in statusJson.EnumerateArray()
                         select new Status(status))
                        .ToList();
                    break;
                case StatusType.Retweeters:
                    statusList = new List<Status>
                    {
                        new Status
                        {
                            Users =
                                (from id in statusJson.GetProperty("ids").EnumerateArray()
                                 select id.GetUInt64())
                                .ToList(),
                            CursorMovement = new Cursors(statusJson)
                        }
                    };
                    break;
                case StatusType.Oembed:
                    statusList = new List<Status>
                    {
                        new Status
                        {
                            EmbeddedStatus = new EmbeddedStatus(statusJson)
                        }
                    };
                    break;
                default:
                    statusList = new List<Status>();
                    break;
            }

            foreach (var status in statusList)
            {
                status.Type = Type;
                status.ID = ID;
                status.UserID = UserID;
                status.ScreenName = ScreenName;
                status.SinceID = SinceID;
                status.MaxID = MaxID;
                status.Count = Count;
                status.Cursor = Cursor;
                status.IncludeRetweets = IncludeRetweets;
                status.ExcludeReplies = ExcludeReplies;
                status.IncludeEntities = IncludeEntities;
                status.IncludeUserEntities = IncludeUserEntities;
                status.TrimUser = TrimUser;
                status.IncludeContributorDetails = IncludeContributorDetails;
                status.IncludeMyRetweet = IncludeMyRetweet;
                status.IncludeAltText = IncludeAltText;
                status.OEmbedAlign = OEmbedAlign;
                status.OEmbedHideMedia = OEmbedHideMedia;
                status.OEmbedHideThread = OEmbedHideThread;
                status.OEmbedMaxWidth = OEmbedMaxWidth;
                status.OEmbedOmitScript = OEmbedOmitScript;
                status.OEmbedRelated = OEmbedRelated;
                status.OEmbedUrl = OEmbedUrl;
                status.OEmbedLanguage = OEmbedLanguage;
                status.TweetIDs = TweetIDs;
                status.Map = Map;
                status.TweetMode = TweetMode;
            }

            return statusList.OfType<T>().ToList();
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement statusJson = JsonDocument.Parse(responseJson).RootElement;

            Status status = ((StatusAction)theAction) switch
            {
                StatusAction.SingleStatus =>
                    new Status(statusJson),
                StatusAction.MediaUpload =>
                    status = new Status
                    {
                        Media = new Media(statusJson)
                    },
                _ => new Status()
            };

            return status.ItemCast(default(T));
        }
    }
}
