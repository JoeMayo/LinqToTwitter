using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Collections;

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

        private IEnumerable<Trend> HandleDailyWeeklyResponse(string responseJson)
        {
            var converter = new Converter();
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { converter });
            var period = jss.Deserialize<JsonDailyWeeklyTrends>(responseJson);
            var asOf = XTwitterElement.EpochBase + TimeSpan.FromSeconds(period.as_of);
            var emptyLocations = new List<Location>();
            var flat = from slot in period.slots
                       let slotTime = slot.time_slot.GetDate(this.Date)
                       let trends = (from trend in slot.trends
                                     select new Trend
                                     {
                                         Type = this.Type,
                                         ExcludeHashtags = this.ExcludeHashtags,
                                         Date = this.Date,
                                         Latitude = this.Latitude,
                                         Longitude = this.Longitude,
                                         WeoID = this.WeoID,
                                         TrendDate = slotTime,
                                         Name = trend.name,
                                         Query = trend.query,
                                         SearchUrl = trend.url,
                                         AsOf = asOf,
                                         Events = trend.events,
                                         PromotedContent = trend.promoted_content,
                                         Location = null,
                                         Locations = emptyLocations
                                     })
                       select trends;

            return flat.SelectMany(trend => trend);
        }

        private IEnumerable<Trend> HandleLocationResponse(string responseJson)
        {
            var now = DateTime.UtcNow;
            var converter = new Converter();
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { converter });
            var responses = jss.Deserialize<JsonTrends[]>(responseJson);
            var flat = from response in responses
                       let asOf = response.as_of.GetDate(now)
                       let locations = (from place in response.locations
                                        select place.ToLocation()).ToList()
                       let trends = (from trend in response.trends
                                     select new Trend
                                     {
                                         Type = this.Type,
                                         ExcludeHashtags = this.ExcludeHashtags,
                                         Date = this.Date,
                                         Latitude = this.Latitude,
                                         Longitude = this.Longitude,
                                         WeoID = this.WeoID,
                                         TrendDate = asOf,
                                         AsOf = asOf,
                                         Name = trend.name,
                                         Query = trend.query,
                                         SearchUrl = trend.url,
                                         Events = trend.events,
                                         PromotedContent = trend.promoted_content,
                                         Location = locations.FirstOrDefault(),
                                         Locations = locations
                                     })
                       select trends;

            return flat.SelectMany(trend => trend);
        }

        private IEnumerable<Trend> HandleAvailableResponse(string responseJson)
        {
            var asOf = DateTime.UtcNow;
            var converter = new Converter();
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { converter });
            var places = jss.Deserialize<JsonPlace[]>(responseJson);
            var locations = new List<Location>();
            foreach (var place in places)
            {
                var location = place.ToLocation();
                locations.Add(location);
            }

            // we fake a single Trend to hang the locations off of...
            yield return new Trend
            {
                Type = this.Type,
                ExcludeHashtags = this.ExcludeHashtags,
                Date = this.Date,
                Name = string.Empty,
                Query = string.Empty,
                SearchUrl = string.Empty,
                TrendDate = asOf,
                AsOf = asOf,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                WeoID = this.WeoID,
                Location = locations.FirstOrDefault(),
                Locations = locations
            };
        }
    }

    internal class Converter : JavaScriptConverter
    {
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new List<Type> 
                {
                    typeof(JsonDailyWeeklyTrends), 
                    typeof(JsonTrends),
                    typeof(JsonSlottedTrend),
                    typeof(JsonTrend),
                    typeof(JsonPlace),
                    typeof(JsonPlaceType)
                };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(JsonDailyWeeklyTrends))
            {
                return JsonDailyWeeklyTrends.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(JsonSlottedTrend))
            {
                return JsonSlottedTrend.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(JsonTrends))
            {
                return JsonTrends.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(JsonTrend))
            {
                return JsonTrend.Deserialize(dictionary, serializer);

            }
            else if (type == typeof(JsonPlace))
            {
                return JsonPlace.Deserialize(dictionary, serializer);
            }
            else if (type == typeof(JsonPlaceType))
            {
                return JsonPlaceType.Deserialize(dictionary, serializer);
            }
            else
            {
                // what?
                return null;
            }
        }
    }

    [DataContract]
    public class JsonDailyWeeklyTrends
    {
        [DataMember]
        public ulong as_of { get; set; } // epoch seconds

        [DataMember]
        public JsonSlottedTrend[] slots { get; set; }

        public static JsonDailyWeeklyTrends Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var trends = dictionary["trends"] as Dictionary<string, object>;
            var asOfEpoch = (int)dictionary["as_of"];
            var trendSlots = from trend in trends
                             let slotTime = trend.Key
                             let trendArray = serializer.ConvertToType<JsonTrend[]>(trend.Value)
                             select new JsonSlottedTrend
                             {
                                 time_slot = slotTime,
                                 trends = trendArray
                             };

            return new JsonDailyWeeklyTrends
            {
                as_of = (ulong)asOfEpoch,
                slots = trendSlots.ToArray()
            };
        }
    }

    [DataContract]
    public class JsonSlottedTrend
    {
        [DataMember]
        public string time_slot { get; set; }

        [DataMember]
        public JsonTrend[] trends { get; set; }

        internal static JsonSlottedTrend Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            // THIS IS WRONG, fortunately we never need to do this...
            var slot = dictionary.First();
            var slotTime = slot.Key;
            var trendArray = serializer.ConvertToType<JsonTrend[]>(slot.Value);
            return new JsonSlottedTrend
            {
                time_slot = slotTime,
                trends = trendArray
            };
        }
    }

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

        internal static JsonTrends Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var jsonTrends = dictionary["trends"] as ArrayList;    // required!
            var createdAt = dictionary.GetValue<string>("created_at");
            var asOf = dictionary.GetValue<string>("as_of");
            var locations = dictionary.GetValue<ArrayList>("locations");
            var locs = (from object location in locations
                        select serializer.ConvertToType<JsonPlace>(location));
            var trds = (from object trend in jsonTrends
                               select serializer.ConvertToType<JsonTrend>(trend));
            return new JsonTrends
            {
                as_of = asOf,
                created_at = createdAt,
                locations = locs.ToArray(),
                trends = trds.ToArray()
            };
        }
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
        [DataMember]
        public string events { get; set; }  // no idea
        [DataMember]
        public string promoted_content { get; set; } // not sure

        internal static JsonTrend Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new JsonTrend
            {
                name = dictionary["name"] as string, // required!
                query = dictionary.GetValue<string>("query"),
                url = dictionary.GetValue<string>("url"),
                events = dictionary.GetValue<string>("events"),
                promoted_content = dictionary.GetValue<string>("promoted_content")
            };
        }
    }

    [DataContract]
    public class JsonPlaceType
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int code { get; set; }

        internal static JsonPlaceType Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            return new JsonPlaceType
            {
                name = dictionary["name"] as string,    // required!
                code = dictionary.GetValue<int>("code")
            };
        }
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

        internal static JsonPlace Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer)
        {
            var placeName = dictionary["name"] as string; // required!
            var pts = dictionary.GetValue<object>("placeType");
            var pt = serializer.ConvertToType<JsonPlaceType>(pts);
            var woeId = dictionary.GetValue<int>("woeid");
            var parentId = dictionary.GetValue<int>("parentid");
            return new JsonPlace
            {
                name = placeName,
                url = dictionary.GetValue<string>("url"),
                woeid = (ulong)woeId,
                parentid = (ulong)parentId,
                placeType = pt,
                country = dictionary.GetValue<string>("country"),
                countryCode = dictionary.GetValue<string>("countryCode")
            };
        }
        
        internal Location ToLocation()
        {
            var pt = this.placeType ?? new JsonPlaceType { code = 0, name = string.Empty };
            return new Location
            {
                Country = this.country ?? string.Empty,
                CountryCode = this.countryCode ?? string.Empty,
                Name = this.name ?? string.Empty,
                ParentID = this.parentid.ToString(CultureInfo.InvariantCulture),
                PlaceTypeName = pt.name,
                PlaceTypeNameCode = pt.code,
                Url = this.url ?? string.Empty,
                WoeID = this.woeid.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
