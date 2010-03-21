using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;

namespace LinqToTwitter
{
    /// <summary>
    /// processes block queries
    /// </summary>
    public class BlocksRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of blocks request to perform
        /// </summary>
        private BlockingType Type { get; set; }

        /// <summary>
        /// id or screen name of user
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name
        /// </summary>
        private ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id
        /// </summary>
        private string ScreenName { get; set; }

        /// <summary>
        /// page to retrieve
        /// </summary>
        public int Page { get; set; }
        
        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Blocks>(
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
        public virtual string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<BlockingType>(parameters["Type"]);

            switch (Type)
            {
                case BlockingType.Blocking:
                    url = BuildBlockingUrl(parameters);
                    break;
                case BlockingType.Exists:
                    url = BuildBlockingExistsUrl(parameters);
                    break;
                case BlockingType.IDS:
                    url = BuildBlockingIDsUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// builds an url for getting blocking ids
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildBlockingIDsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "blocks/blocking/ids.xml";

            return url;
        }

        /// <summary>
        /// builds an url for seeing if a block exists on a user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildBlockingExistsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "blocks/exists.xml";

            url = BuildBlockingExistsUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for getting a list of blocked users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildBlockingUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "blocks/blocking.xml";

            url = BuildBlockingUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Blocking queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildBlockingUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

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
        /// appends parameters for Blocking queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildBlockingExistsUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (!parameters.ContainsKey("ID") && !parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentException("You must specify either ID, UserID, or ScreenName.");
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add("user_id=" + parameters["UserID"]);
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add("screen_name=" + parameters["ScreenName"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        public virtual IList ProcessResults(XElement twitterResponse)
        {
            var blocks = new Blocks
            {
                Type = Type,
                ID = ID,
                UserID = UserID,
                ScreenName = ScreenName,
                Page = Page
            };

            if (twitterResponse.Name == "user")
            {
                blocks.User = new User().CreateUser(twitterResponse);
            }
            else if (twitterResponse.Name == "users")
            {
                var usr = new User();
                blocks.Users =
                    (from user in twitterResponse.Elements("user").ToList()
                     select usr.CreateUser(user))
                     .ToList();
            }
            else if (twitterResponse.Name == "ids")
            {
                blocks.IDs =
                    (from id in twitterResponse.Elements("id").ToList()
                     select id.Value)
                     .ToList();
            }
            else
            {
                throw new ArgumentException("Account Results Processing expected a Twitter response for either a user or hash, but received an unknown element type instead.");
            }

            return new List<Blocks> { blocks };
        }
    }
}
