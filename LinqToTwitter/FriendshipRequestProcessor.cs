using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Friendship queries
    /// </summary>
    class FriendshipRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Friendship>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "SubjectUser",
                       "FollowingUser"
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
                url = BuildFriendshipExistsUrl(parameters);
                return url;
            }

            FriendshipType friendType = RequestProcessorHelper.ParseQueryEnumType<FriendshipType>(parameters["Type"]);

            switch (friendType)
            {
                case FriendshipType.Exists:
                    url = BuildFriendshipExistsUrl(parameters);
                    break;
                default:
                    url = BuildFriendshipExistsUrl(parameters);
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildFriendshipExistsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "friendships/exists.xml";

            url = BuildFriendshipUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Friendship action
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildFriendshipUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("SubjectUser"))
            {
                urlParams.Add("user_a=" + parameters["SubjectUser"]);
            }
            else
            {
                throw new ArgumentException("SubjectUser is required.", "SubjectUser");
            }

            if (parameters.ContainsKey("FollowingUser"))
            {
                urlParams.Add("user_b=" + parameters["FollowingUser"]);
            }
            else
            {
                throw new ArgumentException("FollowingUser is required.", "FollowingUser");
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            var friendList = new List<Friendship>()
            {
                new Friendship { IsFriend = bool.Parse(twitterResponse.Value) }
            };

            return friendList;
        }
    }
}
