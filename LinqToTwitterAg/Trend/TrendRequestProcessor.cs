using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// helps process trend requests
    /// </summary>
    public class TrendRequestProcessor<T>
        : IRequestProcessor<T>
        , IRequestProcessorWithAction<T>
        where T : class
    {
        const string WeoIDParam = "WeoID";

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        internal TrendType Type { get; set; }

        /// <summary>
        /// exclude all trends with hastags if set to true 
        /// (i.e. include "Wolverine" but not "#Wolverine")
        /// </summary>
        bool ExcludeHashtags { get; set; }

        /// <summary>
        /// date to start
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        string Longitude { get; set; }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        int WeoID { get; set; }

        static readonly Dictionary<string, string> worldWoeId;

        static TrendRequestProcessor()
        {
            var worldOnly = worldWoeId = new Dictionary<string, string>();
            worldOnly.Add(WeoIDParam, "1");
            worldWoeId = worldOnly;
        }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Trend>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Date",
                       "ExcludeHashtags",
                       "Latitude",
                       "Longitude",
                       "WeoID"
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
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<TrendType>(parameters["Type"]);

            switch (Type)
            {
                case TrendType.Available:
                    return BuildAvailableTrendsUrl(parameters);
                case TrendType.Place:
                    return BuildPlaceTrendsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds a url for finding trends at a specified location
        /// </summary>
        /// <param name="parameters">parameters should contain WeoID</param>
        /// <returns>base url + location segment</returns>
        private Request BuildPlaceTrendsUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(WeoIDParam))
                throw new ArgumentException("WeoID is a required parameter.", WeoIDParam);

            var req = new Request(BaseUrl + "trends/place.json");
            var urlParams = req.RequestParameters;

            WeoID = int.Parse(parameters[WeoIDParam]);
            urlParams.Add(new QueryParameter("id", parameters[WeoIDParam]));

            return req;
        }

        /// <summary>
        /// Builds an URL for finding where trends are occurring
        /// </summary>
        /// <param name="parameters">parameters can include Latitude and Longitude (must have either both parameter or neither)</param>
        /// <returns>base url + Available segment</returns>
        private Request BuildAvailableTrendsUrl(Dictionary<string, string> parameters)
        {
            if ((parameters.ContainsKey("Latitude") && !parameters.ContainsKey("Longitude")) ||
                (!parameters.ContainsKey("Latitude") && parameters.ContainsKey("Longitude")))
                throw new ArgumentException("If you pass either Latitude or Longitude then you must pass both. Otherwise, don't pass either.", "Latitude/Longitude");

            var req = new Request(BaseUrl + "trends/available.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = parameters["Latitude"];
                urlParams.Add(new QueryParameter("lat", parameters["Latitude"]));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = parameters["Longitude"];
                urlParams.Add(new QueryParameter("long", parameters["Longitude"]));
            }

            return req;
        }

        /// <summary>
        /// appends parameters for trends
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildTrendsUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Date"))
            {
#if NETFX_CORE
                CultureInfo currUiCulture = CultureInfo.CurrentCulture;
#else
                CultureInfo currUiCulture = Thread.CurrentThread.CurrentUICulture;
#endif
                Date = DateTime.Parse(parameters["Date"], currUiCulture);
                urlParams.Add(new QueryParameter("date", Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("ExcludeHashtags") &&
                bool.Parse(parameters["ExcludeHashtags"]))
            {
                ExcludeHashtags = true;
                urlParams.Add(new QueryParameter("exclude", "hashtags"));
            }

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Trend
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Trend</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<Trend> trends = Enumerable.Empty<Trend>();

            if (!string.IsNullOrEmpty(responseJson))
            {
                switch (Type)
                {
                    case TrendType.Available:
                        trends = HandleAvailableResponse(responseJson);
                        break;

                    case TrendType.Place:
                        trends = HandlePlaceResponse(responseJson);
                        break;

                    default:
                        throw new InvalidOperationException("The default case of ProcessResults should never execute because a Type must be specified.");
                }
            }

            return trends.OfType<T>().ToList();
        }

        /// <summary>
        /// transforms json into an action response
        /// </summary>
        /// <param name="responseJson">json with Twitter response</param>
        /// <param name="theAction">Type of action to process</param>
        /// <returns>Action response</returns>
        public virtual T ProcessActionResult(string responseJson, Enum theAction)
        {
            var trend = new Trend();

            return trend.ItemCast(default(T));
        }

        private IEnumerable<Trend> HandlePlaceResponse(string responseJson)
        {
            var responses = JsonMapper.ToObject(responseJson);

            var flat =
                from JsonData response in responses
                let asOf = response.GetValue<string>("as_of").GetDate(DateTime.UtcNow)
                let locations =
                     (from JsonData place in response.GetValue<JsonData>("locations")
                      select new Location(place)).ToList()
                let trends =
                     (from JsonData trend in response.GetValue<JsonData>("trends")
                      select new Trend
                      {
                          Type = Type,
                          ExcludeHashtags = ExcludeHashtags,
                          Date = Date,
                          Latitude = Latitude,
                          Longitude = Longitude,
                          WeoID = WeoID,
                          TrendDate = asOf,
                          AsOf = asOf,
                          Name = trend.GetValue<string>("name"),
                          Query = trend.GetValue<string>("query"),
                          SearchUrl = trend.GetValue<string>("url"),
                          Events = trend.GetValue<string>("events"),
                          PromotedContent = trend.GetValue<string>("promoted_content"),
                          Location = locations.FirstOrDefault(),
                          Locations = locations
                      })
                select trends;

            return flat.SelectMany(trend => trend);
        }

        private IEnumerable<Trend> HandleAvailableResponse(string responseJson)
        {
            var trends = JsonMapper.ToObject(responseJson);
            var locations =
                (from JsonData loc in trends
                 select new Location(loc))
                .ToList();

            var asOf = DateTime.UtcNow;

            // we fake a single Trend to hang the locations off of...
            yield return new Trend
            {
                Type = Type,
                ExcludeHashtags = ExcludeHashtags,
                Date = Date,
                Name = string.Empty,
                Query = string.Empty,
                SearchUrl = string.Empty,
                TrendDate = asOf,
                AsOf = asOf,
                Latitude = Latitude,
                Longitude = Longitude,
                WeoID = WeoID,
                Location = locations.FirstOrDefault(),
                Locations = locations
            };
        }
    }
}
