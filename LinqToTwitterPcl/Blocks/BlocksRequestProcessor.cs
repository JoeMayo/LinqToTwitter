using System;
using System.Collections.Generic;
using System.Linq;

using LinqToTwitter.Common;
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
        /// Removes entities when set to false (true by default)
        /// </summary>
        internal bool IncludeEntities { get; set; }

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
                       "UserID",
                       "ScreenName",
                       "Page",
                       "PerPage",
                       "IncludeEntities",
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
                case BlockingType.List:
                    return BuildListUrl(parameters);
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
            var req = new Request(BaseUrl + "blocks/ids.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// builds an url for getting a list of blocked users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildListUrl(Dictionary<string, string> parameters)
        {
            return BuildBlockingUrlParameters(parameters, "blocks/list.json");
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

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
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
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <returns>List of Blocks</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            var blocks = new Blocks
            {
                Type = Type,
                UserID = UserID,
                ScreenName = ScreenName,
                Page = Page,
                PerPage = PerPage,
                IncludeEntities = IncludeEntities,
                SkipStatus = SkipStatus,
                Cursor = Cursor
            };

            if (string.IsNullOrEmpty(responseJson))
            {
                blocks.Users = new List<User>();
            }
            else
            {
                var blocksJson = JsonMapper.ToObject(responseJson);
                blocks.Cursors = new Cursors(blocksJson);

                switch (Type)
                {
                    case BlockingType.List:
                        HandleList(blocks, blocksJson);
                        break;
                    case BlockingType.Ids:
                        HandleBlockingIDs(blocks, blocksJson);
                        break;
                    default:
                        throw new ArgumentException("Unhandled BlockingType.");
                }
            }

            return new List<Blocks> { blocks }.OfType<T>().ToList();
        }

        void HandleList(Blocks blocks, JsonData blocksJson)
        {
            var users = blocksJson.GetValue<JsonData>("users");

            blocks.Users =
                (from JsonData user in users
                 select new User(user))
                .ToList();
        }

        void HandleBlockingIDs(Blocks blocks, JsonData blocksJson)
        {
            var ids = blocksJson.GetValue<JsonData>("ids");

            blocks.IDs =
                (from JsonData id in ids
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
