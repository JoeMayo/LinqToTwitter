using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

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
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        public TrendType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        public int WoeID { get; set; }

        /// <summary>
        /// Set to true to omit hashtags from results
        /// </summary>
        public bool Exclude { get; set; }

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

            Type = RequestProcessorHelper.ParseEnum<TrendType>(parameters["Type"]);

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
        Request BuildPlaceTrendsUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(WoeIDParam))
                throw new ArgumentException("WoeID is a required parameter.", WoeIDParam);

            var req = new Request(BaseUrl + "trends/place.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            WoeID = int.Parse(parameters[WoeIDParam]);
            urlParams.Add(new QueryParameter("id", parameters[WoeIDParam]));

            return req;
        }

        /// <summary>
        /// Builds an URL for finding where trends are occurring
        /// </summary>
        /// <returns>base url + Available segment</returns>
        Request BuildAvailableTrendsUrl()
        {
            return new Request(BaseUrl + "trends/available.json");
        }

        /// <summary>
        /// Builds an URL for finding trends closest to a lat/long
        /// </summary>
        /// <param name="parameters">parameters can include Latitude and Longitude (must have either both parameters or neither)</param>
        /// <returns>base url + Available segment</returns>
        Request BuildClosestTrendsUrl(Dictionary<string, string> parameters)
        {
            if ((parameters.ContainsKey("Latitude") && !parameters.ContainsKey("Longitude")) ||
                (!parameters.ContainsKey("Latitude") && parameters.ContainsKey("Longitude")))
                throw new ArgumentException("If you pass either Latitude or Longitude then you must pass both. Otherwise, don't pass either.", "Latitude/Longitude");

            var req = new Request(BaseUrl + "trends/closest.json");
            IList<QueryParameter> urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add(new QueryParameter("lat", parameters["Latitude"]));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
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

            if (!string.IsNullOrWhiteSpace(responseJson))
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
        public virtual T? ProcessActionResult(string responseJson, Enum theAction)
        {
            var trend = new Trend();

            return trend.ItemCast(default(T));
        }

        IEnumerable<Trend> HandlePlaceResponse(string responseJson)
        {
            JsonElement responses = JsonDocument.Parse(responseJson).RootElement;

            IEnumerable<IEnumerable<Trend>> flat =
                from response in responses.EnumerateArray()
                let asOf = response.GetString("as_of").GetDate(DateTime.UtcNow)
                let createdAt = response.GetString("created_at").GetDate(DateTime.UtcNow)
                let locations =
                     (from place in response.GetProperty("locations").EnumerateArray()
                      select new Location(place)).ToList()
                let trends =
                     (from trend in response.GetProperty("trends").EnumerateArray()
                      select new Trend
                      {
                          Type = Type,
                          AsOf = asOf,
                          CreatedAt = createdAt,
                          Latitude = Latitude,
                          Longitude = Longitude,
                          WoeID = WoeID,
                          Exclude = Exclude,
                          Name = trend.GetString("name"),
                          Query = trend.GetString("query"),
                          SearchUrl = trend.GetString("url"),
                          Events = trend.GetString("events"),
                          PromotedContent = trend.GetString("promoted_content"),
                          Locations = locations,
                          TweetVolume = trend.GetInt("tweet_volume")
                      })
                select trends;

            return flat.SelectMany(trend => trend);
        }

        IEnumerable<Trend> HandleAvailableOrClosestResponse(string responseJson)
        {
            JsonElement trends = JsonDocument.Parse(responseJson).RootElement;
            List<Location> locations =
                (from loc in trends.EnumerateArray()
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
