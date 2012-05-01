using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LitJson;

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
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of status request, i.e. Show or User
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
        /// Retweets are optional and you must set this to true
        /// before they will be included in the user timeline
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including retweets will return them regardless of the value provided.")]
        public bool IncludeRetweets { get; set; }

        /// <summary>
        /// Don't include replies in responses
        /// </summary>
        public bool ExcludeReplies { get; set; }

        /// <summary>
        /// Include entities in tweets
        /// </summary>
        // TODO: remove after 5/14/12
        [Obsolete("All API methods capable of including entities will return them regardless of the value provided.")]
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Remove all user info, except for User ID
        /// </summary>
        public bool TrimUser { get; set; }

        /// <summary>
        /// Enhances contributor info, beyond the default ID
        /// </summary>
        public bool IncludeContributorDetails { get; set; }

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
                       "Page",
                       "IncludeRetweets",
                       "ExcludeReplies",
                       "IncludeEntities",
                       "TrimUser",
                       "IncludeContributorDetails"
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

            Type = RequestProcessorHelper.ParseQueryEnumType<StatusType>(parameters["Type"]);

            switch (Type)
            {
                case StatusType.Home:
                    return BuildHomeUrl(parameters);
                case StatusType.Mentions:
                    return BuildMentionsUrl(parameters);
                // TODO: Public deprecated on 5/14/12
                //case StatusType.Public:
                //    return BuildPublicUrl();
                case StatusType.Retweets:
                    return BuildRetweetsUrl(parameters);
                case StatusType.RetweetedByMe:
                    return BuildRetweetedByMeUrl(parameters);
                case StatusType.RetweetedToMe:
                    return BuildRetweetedToMeUrl(parameters);
                case StatusType.RetweetsOfMe:
                    return BuildRetweetsOfMeUrl(parameters);
                case StatusType.RetweetedByUser:
                    return BuildRetweetedByUserUrl(parameters);
                case StatusType.RetweetedToUser:
                    return BuildRetweetedToUserUrl(parameters);
                case StatusType.Show:
                    return BuildShowUrl(parameters);
                case StatusType.User:
                    return BuildUserUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildShowUrl(Dictionary<string, string> parameters)
        {
            var url = BuildUrlHelper.TransformIDUrl(parameters, "statuses/show.json");
            return BuildUrlParameters(parameters, url);
        }

        /// <summary>
        /// appends parameters that are common to both friend and user queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
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

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            if (parameters.ContainsKey("IncludeRetweets"))
            {
                IncludeRetweets = bool.Parse(parameters["IncludeRetweets"]);

                if (IncludeRetweets)
                {
                    urlParams.Add(new QueryParameter("include_rts", "true"));
                }
            }

            if (parameters.ContainsKey("ExcludeReplies"))
            {
                ExcludeReplies = bool.Parse(parameters["ExcludeReplies"]);

                if (ExcludeReplies)
                {
                    urlParams.Add(new QueryParameter("exclude_replies", "true"));
                }
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);

                if (IncludeEntities)
                {
                    urlParams.Add(new QueryParameter("include_entities", "true"));
                }
            }

            if (parameters.ContainsKey("TrimUser"))
            {
                TrimUser = bool.Parse(parameters["TrimUser"]);

                if (TrimUser)
                {
                    urlParams.Add(new QueryParameter("trim_user", "true"));
                }
            }

            if (parameters.ContainsKey("IncludeContributorDetails"))
            {
                IncludeContributorDetails = bool.Parse(parameters["IncludeContributorDetails"]);

                if (IncludeContributorDetails)
                {
                    urlParams.Add(new QueryParameter("contributor_details", "true"));
                }
            }

            return req;
        }

        /// <summary>
        /// construct an url for the user timeline
        /// </summary>
        /// <returns>base url + user timeline segment</returns>
        private Request BuildUserUrl(Dictionary<string, string> parameters)
        {
            var url = BuildUrlHelper.TransformIDUrl(parameters, "statuses/user_timeline.json");
            return BuildUrlParameters(parameters, url);
        }

        /// <summary>
        /// construct a base home url
        /// </summary>
        /// <returns>base url + home segment</returns>
        private Request BuildHomeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/home_timeline.json");
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">parameters to build url query with</param>
        /// <returns>base url + mentions segment</returns>
        private Request BuildMentionsUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/mentions.json");
        }

        // TODO: Public deprecated on 5/14/12
        ///// <summary>
        ///// return a public url
        ///// </summary>
        ///// <returns>base url + public segment</returns>
        //private Request BuildPublicUrl()
        //{
        //    return new Request(BaseUrl + "statuses/public_timeline.xml");
        //}

        /// <summary>
        /// construct a url that will request all the retweets of a given tweet
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweet segment</returns>
        private Request BuildRetweetsUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            var url = BuildUrlHelper.TransformIDUrl(parameters, "statuses/retweets.json");
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", Count.ToString(CultureInfo.InvariantCulture)));
            }

            return req;
        }

        /// <summary>
        /// construct a base retweeted by me url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted by me segment</returns>
        private Request BuildRetweetedByMeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweeted_by_me.json");
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted to me segment</returns>
        private Request BuildRetweetedToMeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweeted_to_me.json");
        }

        /// <summary>
        /// construct a base retweeted by user url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted by user segment</returns>
        private Request BuildRetweetedByUserUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweeted_by_user.json");
        }

        /// <summary>
        /// construct a base retweeted to user url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted to user segment</returns>
        private Request BuildRetweetedToUserUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweeted_to_user.json");
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweets of me segment</returns>
        private Request BuildRetweetsOfMeUrl(Dictionary<string, string> parameters)
        {
            return BuildUrlParameters(parameters, "statuses/retweets_of_me.json");
        }

        /// <summary>
        /// transforms Twitter response into List of Status
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of Status</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            JsonData statusJson = JsonMapper.ToObject(responseJson);

            List<Status> statusList;
            switch (Type)
            {
                case StatusType.Show:
                    statusList = new List<Status> { new Status(statusJson) };
                    break;
                case StatusType.Home:
                case StatusType.Mentions:
                case StatusType.RetweetedByMe:
                case StatusType.RetweetedToMe:
                case StatusType.RetweetedToUser:
                case StatusType.RetweetedByUser:
                case StatusType.RetweetsOfMe:
                case StatusType.Retweets:
                case StatusType.User:
                    statusList =
                        (from JsonData status in statusJson
                         select new Status(status))
                        .ToList();
                    break;
                default:
                    statusList = new List<Status>();
                    break;
            }

            statusList.ForEach(
                status =>
                {
                    status.Type = Type;
                    status.ID = ID;
                    status.UserID = UserID;
                    status.ScreenName = ScreenName;
                    status.SinceID = SinceID;
                    status.MaxID = MaxID;
                    status.Count = Count;
                    status.Page = Page;
                    status.IncludeRetweets = IncludeRetweets;
                    status.ExcludeReplies = ExcludeReplies;
                    status.IncludeEntities = IncludeEntities;
                    status.TrimUser = TrimUser;
                    status.IncludeContributorDetails = IncludeContributorDetails;
                });

            return statusList.OfType<T>().ToList();
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData statusJson = JsonMapper.ToObject(responseJson);

            var status = new Status(statusJson);

            return status.ItemCast(default(T));
        }
    }
}
