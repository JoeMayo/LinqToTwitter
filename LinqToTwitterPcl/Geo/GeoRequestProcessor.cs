using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTwitter.Serialization.Extensions;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Geo requests
    /// </summary>
    public class GeoRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T: class
    {
        const string AttributeParam = "Attribute";
        const string IDParam = "ID";

        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of Geo operation (Reverse or ID)
        /// </summary>
        internal GeoType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        internal double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        internal double Longitude { get; set; }

        /// <summary>
        /// IP address to find nearby places
        /// </summary>
        internal string IP { get; set; }

        /// <summary>
        /// How accurate the results should be.
        ///     - A number defaults to meters
        ///     - Default is 0m
        ///     - Feet is ft (as in 10ft)
        /// </summary>
        internal string Accuracy { get; set; }

        /// <summary>
        /// Size of place (i.e. neighborhood is default or city)
        /// </summary>
        internal string Granularity { get; set; }

        /// <summary>
        /// Number of places to return
        /// </summary>
        internal int MaxResults { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        internal string ID { get; set; }

        /// <summary>
        /// Any text you want to add to help find a place
        /// </summary>
        internal string Query { get; set; }

        /// <summary>
        /// Place ID to restrict search to
        /// </summary>
        internal string ContainedWithin { get; set; }

        /// <summary>
        /// Name/value pair separated by "=" (i.e. "street_address=123 4th Street")
        /// </summary>
        internal string Attribute { get; set; }

        /// <summary>
        /// Name of place in similar places query
        /// </summary>
        internal string PlaceName { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return new ParameterFinder<Geo>(
               lambdaExpression.Body,
               new List<string> { 
                   "Type",
                   "Latitude",
                   "Longitude",
                   "IP",
                   "Accuracy",
                   "Granularity",
                   "MaxResults",
                   "ID",
                   "Query",
                   "ContainedWithin",
                   "Attribute",
                   "PlaceName"
               })
               .Parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseQueryEnumType<GeoType>(parameters["Type"]);

            switch (Type)
            {
                case GeoType.ID:
                    return BuildIDUrl(parameters);
                case GeoType.Reverse:
                    return BuildReverseUrl(parameters);
                case GeoType.Search:
                    return BuildSearchUrl(parameters);
                case GeoType.SimilarPlaces:
                    return BuildSimilarPlacesUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        void HandleAttributeParams(Dictionary<string, string> parameters, IList<QueryParameter> urlParams)
        {
            if (parameters.ContainsKey(AttributeParam))
            {
                // TODO: should really be able to search for more than one Attribute
                Attribute = parameters[AttributeParam] ?? String.Empty;
                var parts = Attribute.Split('=');

                if (parts.Length < 2)
                {
                    throw new ArgumentException(
                        "Attribute must be a name/value pair (i.e. street_address=123); actual value: " + Attribute,
                        AttributeParam);
                }

                urlParams.Add(new QueryParameter("attribute:" + parts[0], parts[1]));
            }
        }

        Request BuildSearchUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("IP") &&
                !parameters.ContainsKey("Query") &&
                !(parameters.ContainsKey("Latitude") &&
                  parameters.ContainsKey("Longitude")))
                throw new ArgumentException("Either Latitude and Longitude, Query, or IP address is required.");

            var req = new Request(BaseUrl + "geo/search.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add(new QueryParameter("lat", Latitude.ToString(Culture.US)));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
                urlParams.Add(new QueryParameter("long", Longitude.ToString(Culture.US)));
            }

            if (parameters.ContainsKey("Query"))
            {
                Query = parameters["Query"];
                urlParams.Add(new QueryParameter("query", Query));
            }
            
            if (parameters.ContainsKey("IP"))
            {
                IP = parameters["IP"];
                urlParams.Add(new QueryParameter("ip", IP));
            }

            if (parameters.ContainsKey("Accuracy"))
            {
                Accuracy = parameters["Accuracy"];
                urlParams.Add(new QueryParameter("accuracy", Accuracy));
            }

            if (parameters.ContainsKey("Granularity"))
            {
                Granularity = parameters["Granularity"];
                urlParams.Add(new QueryParameter("granularity", Granularity));
            }

            if (parameters.ContainsKey("MaxResults"))
            {
                MaxResults = int.Parse(parameters["MaxResults"]);
                urlParams.Add(new QueryParameter("max_results", MaxResults.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("ContainedWithin"))
            {
                ContainedWithin = parameters["ContainedWithin"];
                urlParams.Add(new QueryParameter("contained_within", ContainedWithin));
            }

            HandleAttributeParams(parameters, urlParams);

            return req;
        }

        Request BuildIDUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(IDParam))
                throw new ArgumentException("ID is required for a Geo ID query.", IDParam);

            ID = parameters[IDParam];

            var url = "geo/id/" + ID + ".json";
            return new Request(BaseUrl + url);
        }

        Request BuildReverseUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Latitude") || !parameters.ContainsKey("Longitude"))
            {
                const string LatLongParam = "LatLong";
                throw new ArgumentException("Latitude and Longitude parameters are required.", LatLongParam);
            }

            var req = new Request(BaseUrl + "geo/reverse_geocode.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"], CultureInfo.InvariantCulture);
                urlParams.Add(new QueryParameter("lat", double.Parse(parameters["Latitude"]).ToString(Culture.US)));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"], CultureInfo.InvariantCulture);
                urlParams.Add(new QueryParameter("long", double.Parse(parameters["Longitude"]).ToString(Culture.US)));
            }

            if (parameters.ContainsKey("Accuracy"))
            {
                Accuracy = parameters["Accuracy"];
                urlParams.Add(new QueryParameter("accuracy", Accuracy));
            }

            if (parameters.ContainsKey("Granularity"))
            {
                Granularity = parameters["Granularity"];
                urlParams.Add(new QueryParameter("granularity", Granularity));
            }

            if (parameters.ContainsKey("MaxResults"))
            {
                MaxResults = int.Parse(parameters["MaxResults"]);
                urlParams.Add(new QueryParameter("max_results", MaxResults.ToString(CultureInfo.InvariantCulture)));
            }

            return req;
        }

        Request BuildSimilarPlacesUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Latitude") || !parameters.ContainsKey("Longitude"))
            {
                const string LatLongParam = "LatLong";
                throw new ArgumentException("Latitude and Longitude parameters are required.", LatLongParam);
            }

            if (!parameters.ContainsKey("PlaceName"))
            {
                const string LatLongParam = "PlaceName";
                throw new ArgumentException("PlaceName is required.", LatLongParam);
            }

            var req = new Request(BaseUrl + "geo/similar_places.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add(new QueryParameter("lat", Latitude.ToString(Culture.US)));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
                urlParams.Add(new QueryParameter("long", Longitude.ToString(Culture.US)));
            }

            if (parameters.ContainsKey("PlaceName"))
            {
                PlaceName = parameters["PlaceName"];
                urlParams.Add(new QueryParameter("name", PlaceName));
            }

            if (parameters.ContainsKey("ContainedWithin"))
            {
                ContainedWithin = parameters["ContainedWithin"];
                urlParams.Add(new QueryParameter("contained_within", ContainedWithin));
            }

            HandleAttributeParams(parameters, urlParams);

            return req;
        }

        /// <summary>
        /// Transforms response into List of Geo.
        /// </summary>
        /// <param name="responseJson">Json with Twitter response</param>
        /// <returns>List of SavedSearch</returns>
        public List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            JsonData geoJson = JsonMapper.ToObject(responseJson);

            Geo geo;

            switch (Type)
            {
                case GeoType.ID:
                    geo = HandleIDResponse(geoJson);
                    break;
                case GeoType.Reverse:
                case GeoType.Search:
                case GeoType.SimilarPlaces:
                    geo = HandleMultiplePlaceResponse(geoJson);
                    break;
                default:
                    geo = new Geo();
                    break;
            }
                
            return new List<Geo> { geo }.OfType<T>().ToList();
        }
  
        Geo HandleIDResponse(JsonData placeJson)
        {
            var sb = new StringBuilder();
            var writer = new JsonWriter(sb);

            writer.WriteObjectStart();

                writer.WritePropertyName("result");
                    writer.WriteObjectStart();

                        writer.WritePropertyName("places");
                            writer.WriteArrayStart();

                                writer.WriteJsonData(placeJson);

                            writer.WriteArrayEnd();

                    writer.WriteObjectEnd();

            writer.WriteObjectEnd();

            var geoJson = JsonMapper.ToObject(sb.ToString());

            return HandleMultiplePlaceResponse(geoJson);
        }
  
        Geo HandleMultiplePlaceResponse(JsonData geoJson)
        {
            var geo =
                new Geo(geoJson)
                {
                    Type = Type,
                    Accuracy = Accuracy,
                    Granularity = Granularity,
                    ID = ID,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    IP = IP,
                    MaxResults = MaxResults,
                    Query = Query,
                    ContainedWithin = ContainedWithin,
                    Attribute = Attribute,
                    PlaceName = PlaceName
                };
            return geo;
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData geoJson = JsonMapper.ToObject(responseJson);

            switch ((GeoAction)theAction)
            {
                case GeoAction.CreatePlace:
                    var place = new Place(geoJson);
                    return place.ItemCast(default(T));
                default:
                    throw new InvalidOperationException("Unknown Action.");
            }
        }
    }
}
