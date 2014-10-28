using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Social Graph Requests and responses
    /// </summary>
    class SocialGraphRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of request
        /// </summary>
        internal SocialGraphType Type { get; set; }

        /// <summary>
        /// Specfies the ID of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid user ID is also a valid screen name. 
        /// </summary>
        internal ulong UserID { get; set; }

        /// <summary>
        /// Specfies the screen name of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid screen name is also a user ID.
        /// </summary>
        internal string ScreenName { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        internal string Cursor { get; set; }

        /// <summary>
        /// Number of ids to return for each request (max: 5000)
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<SocialGraph>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "UserID",
                       "ScreenName",
                       "Cursor",
                       "Count"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<SocialGraphType>(parameters["Type"]);

            switch (Type)
            {
                case SocialGraphType.Followers:
                    return BuildSocialGraphFollowersUrl(parameters);
                case SocialGraphType.Friends:
                    return BuildSocialGraphFriendsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSocialGraphFriendsUrl(Dictionary<string, string> parameters)
        {
            var url = "friends/ids.json";

            return BuildSocialGraphUrlParameters(parameters, url);
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSocialGraphFollowersUrl(Dictionary<string, string> parameters)
        {
            var url = "followers/ids.json";

            return BuildSocialGraphUrlParameters(parameters, url);
        }

        /// <summary>
        /// appends parameters for Friendship action
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildSocialGraphUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (!parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
                throw new ArgumentException("You must specify either UserID or ScreenName.");

            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

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

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }
            else
            {
                Cursor = "-1";
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            return req;
        }

        /// <summary>
        /// Transforms Twitter response into List of SocialGraph
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of SocialGraph</returns>
        public List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            JsonData graphJson = JsonMapper.ToObject(responseJson);

            var graph = new SocialGraph
            {
                Type = Type,
                UserID = UserID,
                ScreenName = ScreenName,
                Cursor = Cursor,
                Count = Count,
                CursorMovement = new Cursors(graphJson)
            };

            switch (Type)
            {
                case SocialGraphType.Friends:
                case SocialGraphType.Followers:
                    graph.IDs =
                        (from JsonData id in graphJson.GetValue<JsonData>("ids")
                         select id.ToString())
                        .ToList();
                    break;
                default:
                    break;
            }

            return new List<SocialGraph> { graph }.OfType<T>().ToList();
        }
    }
}
