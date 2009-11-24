using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Status requests
    /// </summary>
    public class StatusRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

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
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
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
                       "Page"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                url = BuildPublicUrl();
                return url;
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<StatusType>(parameters["Type"]);

            switch (Type)
            {
                case StatusType.Friends:
                    url = BuildFriendUrl(parameters);
                    break;
                case StatusType.Home:
                    url = BuildHomeUrl(parameters);
                    break;
                case StatusType.Mentions:
                    url = BuildMentionsUrl(parameters);
                    break;
                case StatusType.Public:
                    url = BuildPublicUrl();
                    break;
                case StatusType.Retweets:
                    url = BuildRetweetsUrl(parameters);
                    break;
                case StatusType.RetweetedByMe:
                    url = BuildRetweetedByMeUrl(parameters);
                    break;
                case StatusType.RetweetedToMe:
                    url = BuildRetweetedToMeUrl(parameters);
                    break;
                case StatusType.RetweetsOfMe:
                    url = BuildRetweetsOfMeUrl(parameters);
                    break;
                case StatusType.Show:
                    url = BuildShowUrl(parameters);
                    break;
                case StatusType.User:
                    url = BuildUserUrl(parameters);
                    break;
                default:
                    url = BuildPublicUrl();
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildShowUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            var url = BaseUrl + "statuses/show.xml";

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters that are common to both friend and user queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildFriendRepliesAndUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add("max_id=" + parameters["MaxID"]);
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add("count=" + parameters["Count"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// construct a base user url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + user timeline segment</returns>
        private string BuildUserUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/user_timeline.xml";

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// construct a base friend url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + friend segment</returns>
        private string BuildFriendUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/friends_timeline.xml";
            
            url = BuildFriendRepliesAndUrlParameters(parameters, url);
            
            return url;
        }

        /// <summary>
        /// construct a base home url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + home segment</returns>
        private string BuildHomeUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/home_timeline.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + friend segment</returns>
        private string BuildMentionsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/mentions.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// return a public url
        /// </summary>
        /// 
        /// <returns>base url + public segment</returns>
        private string BuildPublicUrl()
        {
            var url = BaseUrl + "statuses/public_timeline.xml";
            return url;
        }

        /// <summary>
        /// construct a url that will request all the retweets of a given tweet
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweet segment</returns>
        private string BuildRetweetsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/retweets.xml";

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
            }

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                url += "?count=" + Count;
            }

            return url;
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted by me segment</returns>
        private string BuildRetweetedByMeUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/retweeted_by_me.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweeted to me segment</returns>
        private string BuildRetweetedToMeUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/retweeted_to_me.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// construct a base mentions url
        /// </summary>
        /// <param name="parameters">input parameters</param>
        /// <returns>base url + retweets of me segment</returns>
        private string BuildRetweetsOfMeUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/retweets_of_me.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of Status
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of Status</returns>
        public IList ProcessResults(XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("status").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "status")
            {
                responseItems.Add(twitterResponse);
            }

            var usr = new User();

            var statusList =
                from status in responseItems
                let dateParts =
                    status.Element("created_at").Value.Split(' ')
                let createdAtDate =
                    dateParts.Count() > 1 ?
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT",
                        dateParts[1],
                        dateParts[2],
                        dateParts[5],
                        dateParts[3]),
                        CultureInfo.InvariantCulture) :
                    DateTime.MinValue
                let user = status.Element("user")
                let retweet = status.Element("retweeted_status")
                let rtDateParts =
                    retweet == null ? 
                        null :
                        retweet.Element("created_at").Value.Split(' ')
                let retweetedAtDate =
                    retweet == null ?
                        DateTime.MinValue :
                        DateTime.Parse(
                            string.Format("{0} {1} {2} {3} GMT",
                            rtDateParts[1],
                            rtDateParts[2],
                            rtDateParts[5],
                            rtDateParts[3]),
                            CultureInfo.InvariantCulture)
                select
                   new Status
                   {
                       Type = Type,
                       ID = ID,
                       UserID = UserID,
                       ScreenName = ScreenName,
                       Page = Page,
                       Count = Count,
                       MaxID = MaxID,
                       SinceID = SinceID,
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

            return statusList.ToList();
        }
    }
}
