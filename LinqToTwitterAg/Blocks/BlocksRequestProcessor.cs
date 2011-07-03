using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes block queries
    /// </summary>
    public class BlocksRequestProcessor<T> : IRequestProcessor<T>
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
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<BlockingType>(parameters["Type"]);

            switch (Type)
            {
                case BlockingType.Blocking:
                    return BuildBlockingUrl(parameters);
                case BlockingType.Exists:
                    return BuildBlockingExistsUrl(parameters);
                case BlockingType.IDS:
                    return BuildBlockingIDsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for getting blocking ids
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildBlockingIDsUrl(Dictionary<string, string> parameters)
        {
           return new Request(BaseUrl + "blocks/blocking/ids.xml");
        }

        /// <summary>
        /// builds an url for seeing if a block exists on a user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildBlockingExistsUrl(Dictionary<string, string> parameters)
        {
            return BuildBlockingExistsUrlParameters(parameters, "blocks/exists.xml");

        }

        /// <summary>
        /// builds an url for getting a list of blocked users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildBlockingUrl(Dictionary<string, string> parameters)
        {
            return BuildBlockingUrlParameters(parameters, "blocks/blocking.xml");
        }

        /// <summary>
        /// appends parameters for Blocking queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildBlockingUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            return req;
        }

        /// <summary>
        /// appends parameters for Blocking queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildBlockingExistsUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (!parameters.ContainsKey("ID") && !parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
                throw new ArgumentException("You must specify either ID, UserID, or ScreenName.");

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

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

            return req;
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <param name="responseXML">XML response from Twitter</param>
        /// <returns>List of Blocks</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
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
                blocks.User = User.CreateUser(twitterResponse);
            }
            else if (twitterResponse.Name == "users")
            {
                blocks.Users =
                    (from user in twitterResponse.Elements("user").ToList()
                     select User.CreateUser(user))
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

            return new List<Blocks>{ blocks }.OfType<T>().ToList();
        }
    }
}
