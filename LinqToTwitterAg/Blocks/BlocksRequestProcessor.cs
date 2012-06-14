using System;
using System.Collections.Generic;
using System.Linq;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes block queries
    /// </summary>
    public class BlocksRequestProcessor<T> :
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
        /// type of blocks request to perform
        /// </summary>
        internal BlockingType Type { get; set; }

        /// <summary>
        /// id or screen name of user
        /// </summary>
        internal string ID { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name
        /// </summary>
        internal ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id
        /// </summary>
        internal string ScreenName { get; set; }

        /// <summary>
        /// page to retrieve
        /// </summary>
        internal int Page { get; set; }

        /// <summary>
        /// Number of items per page to return (input only)
        /// </summary>
        internal int PerPage { get; set; }

        /// <summary>
        /// Don't include statuses in response (input only)
        /// </summary>
        internal bool SkipStatus { get; set; }

        /// <summary>
        /// Identifier for previous or next page to query (input only)
        /// </summary>
        internal string Cursor { get; set; }

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
                       "Page",
                       "PerPage",
                       "SkipStatus",
                       "Cursor"
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

            Type = RequestProcessorHelper.ParseQueryEnumType<BlockingType>(parameters["Type"]);

            switch (Type)
            {
                case BlockingType.Blocking:
                    return BuildBlockingUrl(parameters);
                case BlockingType.Exists:
                    return BuildBlockingExistsUrl(parameters);
                case BlockingType.Ids:
                    return BuildBlockingIDsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for getting blocking ids
        /// </summary>
        /// <returns>base url + show segment</returns>
        Request BuildBlockingIDsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "blocks/blocking/ids.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
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

            if (parameters.ContainsKey("PerPage"))
            {
                PerPage = int.Parse(parameters["PerPage"]);
                urlParams.Add(new QueryParameter("per_page", parameters["PerPage"]));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
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

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
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
                Page = Page,
                PerPage = PerPage,
                SkipStatus = SkipStatus,
                Cursor = Cursor
            };

            switch (Type)
            {
                case BlockingType.Blocking:
                    HandleBlocking(blocks, blocksJson);
                    break;
                case BlockingType.Exists:
                    HandleBlockingExists(blocks, blocksJson);
                    break;
                case BlockingType.Ids:
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

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData blocksJson = JsonMapper.ToObject(responseJson);

            var user = new User(blocksJson);

            return user.ItemCast(default(T));
        }
    }
}
