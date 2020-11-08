using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

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
        public BlockingType Type { get; set; }

        /// <summary>
        /// disambiguates when user id is screen name
        /// </summary>
        public ulong UserID { get; set; }

        /// <summary>
        /// disambiguates when screen name is user id
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// page to retrieve
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page to return (input only)
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Don't include statuses in response (input only)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Identifier for previous or next page to query (input only)
        /// </summary>
        public string Cursor { get; set; }

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

            return paramFinder.Parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<BlockingType>(parameters["Type"]);

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
            var req = new Request(BaseUrl + "blocks/list.json");
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

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                blocks.Users = new List<User>();
            }
            else
            {
                var blocksJson = JsonDocument.Parse(responseJson).RootElement;
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

        void HandleList(Blocks blocks, JsonElement blocksJson)
        {
            var users = blocksJson.GetProperty("users");

            blocks.Users =
                (from user in users.EnumerateArray()
                 select new User(user))
                .ToList();
        }

        void HandleBlockingIDs(Blocks blocks, JsonElement blocksJson)
        {
            var ids = blocksJson.GetProperty("ids");

            blocks.IDs =
                (from id in ids.EnumerateArray()
                 select id.ToString())
                .ToList();
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement blocksJson = JsonDocument.Parse(responseJson).RootElement;

            var user = new User(blocksJson);

            return user.ItemCast(default(T));
        }
    }
}
