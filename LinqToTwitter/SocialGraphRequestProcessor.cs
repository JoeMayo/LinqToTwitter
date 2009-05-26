using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace LinqToTwitter
{
    class SocialGraphRequestProcessor : IRequestProcessor
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
               new ParameterFinder<SocialGraph>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "UserID",
                       "ScreenName",
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
                url = BuildSocialGraphFriendsUrl(parameters);
                return url;
            }

            switch ((SocialGraphType)Enum.ToObject(typeof(SocialGraphType), int.Parse(parameters["Type"])))
            {
                case SocialGraphType.Followers:
                    url = BuildSocialGraphFollowersUrl(parameters);
                    break;

                default:
                    url = BuildSocialGraphFriendsUrl(parameters);
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildSocialGraphFriendsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "friends/ids.xml";

            url = BuildSocialGraphUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildSocialGraphFollowersUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "followers/ids.xml";

            url = BuildSocialGraphUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Friendship action
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildSocialGraphUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (!parameters.ContainsKey("ID") && !parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("You must specify either ID, UserID, or ScreenName.");
            }

            if (parameters.ContainsKey("ID"))
            {
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            if (parameters.ContainsKey("UserID"))
            {
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                urlParams.Add("page=" + parameters["Page"]);
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
            var idList =
                from id in twitterResponse.Elements("id").ToList()
                select new SocialGraph
                {
                    ID = int.Parse(id.Value)
                };

            return idList.ToList();
            //return idList.AsQueryable<SocialGraph>();
        }
    }
}
