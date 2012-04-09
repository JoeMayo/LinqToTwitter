using System;
using System.Collections.Generic;
using System.Linq;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes block queries
    /// </summary>
    public class BlocksRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of blocks request to perform
        /// </summary>
        internal BlockingType Type { get; set; }

        /// <summary>
        /// id or screen name of user
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name
        /// </summary>
        ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id
        /// </summary>
        string ScreenName { get; set; }

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
            const string typeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", typeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<BlockingType>(parameters["Type"]);

            switch (Type)
            {
                case BlockingType.Blocking:
                    return BuildBlockingUrl(parameters);
                case BlockingType.Exists:
                    return BuildBlockingExistsUrl(parameters);
                case BlockingType.IDS:
                    return BuildBlockingIDsUrl();
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for getting blocking ids
        /// </summary>
        /// <returns>base url + show segment</returns>
        Request BuildBlockingIDsUrl()
        {
           return new Request(BaseUrl + "blocks/blocking/ids.json");
        }

        /// <summary>
        /// builds an url for seeing if a block exists on a user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildBlockingExistsUrl(Dictionary<string, string> parameters)
        {
            return BuildBlockingExistsUrlParameters(parameters, "blocks/exists.json");
        }

        /// <summary>
        /// builds an url for getting a list of blocked users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildBlockingUrl(Dictionary<string, string> parameters)
        {
            return BuildBlockingUrlParameters(parameters, "blocks/blocking.json");
        }

        /// <summary>
        /// appends parameters for Blocking queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildBlockingUrlParameters(Dictionary<string, string> parameters, string url)
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
        Request BuildBlockingExistsUrlParameters(Dictionary<string, string> parameters, string url)
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
        /// <returns>List of Blocks</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {

            var blocksJson = JsonMapper.ToObject(responseJson);

            var blocks = new Blocks
            {
                Type = Type,
                ID = ID,
                UserID = UserID,
                ScreenName = ScreenName,
                Page = Page
            };

            switch (Type)
            {
                case BlockingType.Blocking:
                    HandleBlocking(blocks, blocksJson);
                    break;
                case BlockingType.Exists:
                    HandleBlockingExists(blocks, blocksJson);
                    break;
                case BlockingType.IDS:
                    HandleBlockingIDs(blocks, blocksJson);
                    break;
                default:
                    throw new ArgumentException("Unhandled BlockingType.");
            }

            return new List<Blocks> { blocks }.OfType<T>().ToList();
        }

        private void HandleBlocking(Blocks blocks, JsonData blocksJson)
        {
            blocks.Users =
                (from JsonData user in blocksJson
                 select new User(user))
                .ToList();
        }

        private void HandleBlockingExists(Blocks blocks, JsonData blocksJson)
        {
            blocks.User = new User(blocksJson);
        }

        private void HandleBlockingIDs(Blocks blocks, JsonData blocksJson)
        {
            blocks.IDs =
                (from JsonData id in blocksJson
                 select id.ToString())
                .ToList();
        }
    }
}
