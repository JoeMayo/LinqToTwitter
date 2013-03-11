using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        const string WoeIDParam = "WoeID";

        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        internal TrendType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        internal string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        internal string Longitude { get; set; }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        internal int WoeID { get; set; }

        /// <summary>
        /// Set to true to omit hashtags from results
        /// </summary>
        internal bool Exclude { get; set; }

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
                       "Latitude",
                       "Longitude",
                       "WoeID",
                       "Exclude"
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
                    return BuildAvailableTrendsUrl();
                case TrendType.Closest:
                    return BuildClosestTrendsUrl(parameters);
                case TrendType.Place:
                    return BuildPlaceTrendsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds a url for finding trends at a specified location
        /// </summary>
        /// <param name="parameters">parameters should contain WoeID</param>
        /// <returns>base url + location segment</returns>
        private Request BuildPlaceTrendsUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(WoeIDParam))
                throw new ArgumentException("WoeID is a required parameter.", WoeIDParam);

            var req = new Request(BaseUrl + "trends/place.json");
            var urlParams = req.RequestParameters;

            WoeID = int.Parse(parameters[WoeIDParam]);
            urlParams.Add(new QueryParameter("id", parameters[WoeIDParam]));

            return req;
        }

        /// <summary>
        /// Builds an URL for finding where trends are occurring
        /// </summary>
        /// <returns>base url + Available segment</returns>
        private Request BuildAvailableTrendsUrl()
        {
            return new Request(BaseUrl + "trends/available.json");
        }

        /// <summary>
        /// Builds an URL for finding trends closest to a lat/long
        /// </summary>
        /// <param name="parameters">parameters can include Latitude and Longitude (must have either both parameters or neither)</param>
        /// <returns>base url + Available segment</returns>
        private Request BuildClosestTrendsUrl(Dictionary<string, string> parameters)
        {
            if ((parameters.ContainsKey("Latitude") && !parameters.ContainsKey("Longitude")) ||
                (!parameters.ContainsKey("Latitude") && parameters.ContainsKey("Longitude")))
                throw new ArgumentException("If you pass either Latitude or Longitude then you must pass both. Otherwise, don't pass either.", "Latitude/Longitude");

            var req = new Request(BaseUrl + "trends/closest.json");
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
                    case TrendType.Closest:
                        trends = HandleAvailableOrClosestResponse(responseJson);
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

        IEnumerable<Trend> HandlePlaceResponse(string responseJson)
        {
            var responses = JsonMapper.ToObject(responseJson);

            var flat =
                from JsonData response in responses
                let asOf = response.GetValue<string>("as_of").GetDate(DateTime.UtcNow)
                let createdAt = response.GetValue<string>("created_at").GetDate(DateTime.UtcNow)
                let locations =
                     (from JsonData place in response.GetValue<JsonData>("locations")
                      select new Location(place)).ToList()
                let trends =
                     (from JsonData trend in response.GetValue<JsonData>("trends")
                      select new Trend
                      {
                          Type = Type,
                          AsOf = asOf,
                          CreatedAt = createdAt,
                          Latitude = Latitude,
                          Longitude = Longitude,
                          WoeID = WoeID,
                          Exclude = Exclude,
                          Name = trend.GetValue<string>("name"),
                          Query = trend.GetValue<string>("query"),
                          SearchUrl = trend.GetValue<string>("url"),
                          Events = trend.GetValue<string>("events"),
                          PromotedContent = trend.GetValue<string>("promoted_content"),
                          Locations = locations
                      })
                select trends;

            return flat.SelectMany(trend => trend);
        }

        IEnumerable<Trend> HandleAvailableOrClosestResponse(string responseJson)
        {
            var trends = JsonMapper.ToObject(responseJson);
            var locations =
                (from JsonData loc in trends
                 select new Location(loc))
                .ToList();

            // we fake a single Trend to hang the locations off of...
            yield return new Trend
            {
                Type = Type,
                AsOf = DateTime.UtcNow,
                Latitude = Latitude,
                Longitude = Longitude,
                WoeID = WoeID,
                Exclude = Exclude,
                Locations = locations
            };
        }
    }
}
