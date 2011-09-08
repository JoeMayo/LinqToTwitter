using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// helps process trend requests
    /// </summary>
    public class TrendRequestProcessor<T>
        : IRequestProcessor<T>
        , IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        private TrendType Type { get; set; }

        /// <summary>
        /// exclude all trends with hastags if set to true 
        /// (i.e. include "Wolverine" but not "#Wolverine")
        /// </summary>
        private bool ExcludeHashtags { get; set; }

        /// <summary>
        /// date to start
        /// </summary>
        private DateTime Date { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        private string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        private string Longitude { get; set; }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        private int WeoID { get; set; }

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

        static TrendRequestProcessor()
        {
            var worldOnly = s_WorldWoeId = new Dictionary<string, string>();
            worldOnly.Add("WeoID", "1");
            s_WorldWoeId = worldOnly;
        }

        private static readonly Dictionary<string, string> s_WorldWoeId;

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<TrendType>(parameters["Type"]);

            switch (Type)
            {
                case TrendType.Trend:
                    return BuildLocationTrendsUrl(s_WorldWoeId);
                case TrendType.Daily:
                    return BuildDailyTrendsUrl(parameters);
                case TrendType.Weekly:
                    return BuildWeeklyTrendsUrl(parameters);
                case TrendType.Available:
                    return BuildAvailableTrendsUrl(parameters);
                case TrendType.Location:
                    return BuildLocationTrendsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds a url for finding trends at a specified location
        /// </summary>
        /// <param name="parameters">parameters should contain WeoID</param>
        /// <returns>base url + location segment</returns>
        private Request BuildLocationTrendsUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("WeoID"))
                throw new ArgumentException("WeoID is a required parameter.", "WeoID");

            WeoID = int.Parse(parameters["WeoID"]);
            var url = "trends/" + parameters["WeoID"] + ".json";

            return new Request(BaseUrl + url);
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
                throw new ArgumentException("If you pass either Latitude or Longitude then you must pass both. Otherwise, don't pass either.");

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
        /// builds an url for showing daily trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildDailyTrendsUrl(Dictionary<string, string> parameters)
        {
            return BuildTrendsUrlParameters(parameters, "trends/daily.json");
        }

        /// <summary>
        /// builds an url for showing weekly trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildWeeklyTrendsUrl(Dictionary<string, string> parameters)
        {
            return BuildTrendsUrlParameters(parameters, "trends/weekly.json");
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
                Date = DateTime.Parse(parameters["Date"], CultureInfo.InvariantCulture);
                urlParams.Add(new QueryParameter("date", Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("ExcludeHashtags") &&
                bool.Parse(parameters["ExcludeHashtags"]) == true)
            {
                ExcludeHashtags = true;
                urlParams.Add(new QueryParameter("exclude", "hashtags"));
            }

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Trend
        /// </summary>
        /// <param name="responseXml">XML response from Twitter</param>
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

                    case TrendType.Daily:
                    case TrendType.Weekly:
                        trends = HandleDailyWeeklyResponse(responseJson);
                        break;

                    case TrendType.Location:
                    case TrendType.Trend:
                        trends = HandleLocationResponse(responseJson);
                        break;

                    default:
                        throw new InvalidOperationException("The default case of ProcessResults should never execute because a Type must be specified.");
                }
            }

            return trends.OfType<T>().ToList();
        }

        private static readonly DateTime s_EpochBase = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private IEnumerable<Trend> HandleDailyWeeklyResponse(string responseJson)
        {
#if TwitterWasSane
            var now = DateTime.UtcNow;
            var emptyLocations = new List<Location>();
            var period = responseJson.Deserialize<JsonDailyWeeklyTrends>();
            var asOf = s_EpochBase + TimeSpan.FromSeconds(period.as_of);
            var flat = from slot in period.slots
                       let hour = slot.hour_slot.GetDate(Date)
                       let trends = (from trend in slot.trends
                                     select new Trend
                                     {
                                         Type = Type,
                                         ExcludeHashtags = ExcludeHashtags,
                                         Date = Date,
                                         Name = trend.name,
                                         Query = trend.query,
                                         SearchUrl = trend.url,
                                         AsOf = asOf,
                                         Latitude = Latitude,
                                         Longitude = Longitude,
                                         WeoID = WeoID,
                                         Location = null,
                                         Locations = emptyLocations
                                     })
                       select trends;

            return flat.SelectMany(trend => trend);
#endif
            return Enumerable.Empty<Trend>();
        }

        private IEnumerable<Trend> HandleLocationResponse(string responseJson)
        {
            var now = DateTime.UtcNow;
            var responses = responseJson.DeserializeArray<JsonTrends>();
            var flat = from response in responses
                      let asOf = response.as_of.GetDate(now)
                      let locations = (from place in response.locations
                                       select place.ToLocation()).ToList()
                      let trends = (from trend in response.trends
                                    select new Trend
                                    {
                                        Type = Type,
                                        ExcludeHashtags = ExcludeHashtags,
                                        Date = Date,
                                        Name = trend.name,
                                        Query = trend.query,
                                        SearchUrl = trend.url,
                                        AsOf = asOf,
                                        Latitude = Latitude,
                                        Longitude = Longitude,
                                        WeoID = WeoID,
                                        Location = locations.FirstOrDefault(),
                                        Locations = locations
                                    })
                      select trends;

            return flat.SelectMany(trend => trend);
        }

        private IEnumerable<Trend> HandleAvailableResponse(string responseJson)
        {
            var asOf = DateTime.UtcNow;
            var places = responseJson.DeserializeArray<JsonPlace>();
            var locations = new List<Location>();
            foreach (var place in places)
            {
                var location = place.ToLocation();
                locations.Add(location);
            }

            // we fake a single Trend to hang the locations off of...
            yield return new Trend
            {
                Type = Type,
                ExcludeHashtags = ExcludeHashtags,
                Date = Date,
                Name = string.Empty,
                Query = string.Empty,
                SearchUrl = string.Empty,
                AsOf = asOf,
                Latitude = Latitude,
                Longitude = Longitude,
                WeoID = WeoID,
                Location = locations.FirstOrDefault(),
                Locations = locations
            };
        }
    }

#if TwitterWasSane
    // this is what the returned json SHOULD look like, but doesn't
    [DataContract]
    public class JsonSlottedTrend
    {
        [DataMember]
        public string hour_slot { get; set; }

        [DataMember]
        public JsonTrend[] trends { get; set; }
    }

    [DataContract]
    public class JsonDailyWeeklyTrends
    {
        [DataMember]
        public JsonSlottedTrend[] slots { get; set; }

        [DataMember]
        public int as_of { get; set; } // epoch seconds
    }
#endif

    [DataContract]
    public class JsonTrends
    {
        [DataMember]
        public JsonTrend[] trends { get; set; }
        [DataMember]
        public string created_at { get; set; }
        [DataMember]
        public string as_of { get; set; }
        [DataMember]
        public JsonPlace[] locations { get; set; }
    }

    [DataContract]
    public class JsonTrend
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string url { get; set; }
#if UNSUPPORTEDBYLinqToTwitter // so don't bother parsing these...
        [DataMember]
        public JsonEvent[] events { get; set; }
        [DataMember]
        public bool promoted_content { get; set; } // not sure
#endif
    }

    [DataContract]
    public class JsonPlaceType
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string code { get; set; }
    }

    [DataContract]
    public class JsonPlace
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public ulong woeid { get; set; }
        [DataMember]
        public ulong parentid { get; set; }
        [DataMember]
        public JsonPlaceType placeType { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string countryCode { get; set; }

        internal Location ToLocation()
        {
            var pt = this.placeType ?? new JsonPlaceType { code = "0", name = string.Empty };
            return new Location
            {
                Country = this.country ?? string.Empty,
                CountryCode = this.countryCode ?? string.Empty,
                Name = this.name ?? string.Empty,
                ParentID = this.parentid.ToString(CultureInfo.InvariantCulture),
                PlaceTypeName = pt.name,
                PlaceTypeNameCode = int.Parse(pt.code),
                Url = this.url ?? string.Empty,
                WoeID = this.woeid.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
