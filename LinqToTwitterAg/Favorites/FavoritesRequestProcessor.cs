using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Manages request processing for favorites
    /// </summary>
    public class FavoritesRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of favorites to query
        /// </summary>
        private FavoritesType Type { get; set; }

        /// <summary>
        /// User identity to search (optional)
        /// </summary>
        private string UserID { get; set; }

        /// <summary>
        /// Screen name of user to search (optional)
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Number of items to return in a single request (optional)
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Start search at this ID (optional)
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// Don't return results past this ID (optional)
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// Page to retrieve (optional)
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// Add entities to results (optional)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Favorites>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "UserID",
                       "ScreenName",
                       "Count",
                       "SinceID",
                       "MaxID",
                       "Page",
                       "IncludeEntities"
                   })
                   .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            const string ParamsParam = "parameters";

            if (parameters == null)
                throw new ArgumentException("You must pass a \"parameters\" Dictionary<string, string> parameter.", ParamsParam);

            if (!parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<FavoritesType>(parameters["Type"]);

            return BuildFavoritesUrlParameters(parameters);
        }

        /// <summary>
        /// appends parameters for Favorites request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildFavoritesUrlParameters(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "favorites/list.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                SinceID = ulong.Parse(parameters["SinceID"]);
                urlParams.Add(new QueryParameter("since_id", parameters["SinceID"]));
            }

            if (parameters.ContainsKey("MaxID"))
            {
                MaxID = ulong.Parse(parameters["MaxID"]);
                urlParams.Add(new QueryParameter("max_id", parameters["MaxID"]));
            }

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add(new QueryParameter("page", parameters["Page"]));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_entities", parameters["IncludeEntities"].ToLower()));
            }

            return req;
        }


        /// <summary>
        /// transforms Twitter response into list of Favorites
        /// </summary>
        /// <param name="responseJson">JSON with Twitter response</param>
        /// <returns>List of Favorite</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            JsonData favoritesJson = JsonMapper.ToObject(responseJson);

            var statusList =
                from JsonData favJson in favoritesJson
                select new Favorites(favJson)
                {
                    Type = Type,
                    UserID = UserID,
                    ScreenName = ScreenName,
                    Count = Count,
                    SinceID = SinceID,
                    MaxID = MaxID,
                    Page = Page,
                    IncludeEntities = IncludeEntities
                };

            return statusList.OfType<T>().ToList();
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData statusJson = JsonMapper.ToObject(responseJson);

            var status = new Status(statusJson);

            return status.ItemCast(default(T));
        }
    }
}
