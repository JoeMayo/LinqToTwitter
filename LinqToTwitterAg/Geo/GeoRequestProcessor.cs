using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Saved Search requests
    /// </summary>
    public class GeoRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of Geo operation (Reverse or ID)
        /// </summary>
        private GeoType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        private double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        private double Longitude { get; set; }

        /// <summary>
        /// IP address to find nearby places
        /// </summary>
        private string IP { get; set; }

        /// <summary>
        /// How accurate the results should be.
        ///     - A number defaults to meters
        ///     - Default is 0m
        ///     - Feet is ft (as in 10ft)
        /// </summary>
        private string Accuracy { get; set; }

        /// <summary>
        /// Size of place (i.e. neighborhood is default or city)
        /// </summary>
        private string Granularity { get; set; }

        /// <summary>
        /// Number of places to return
        /// </summary>
        private int MaxResults { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        /// Any text you want to add to help find a place
        /// </summary>
        private string Query { get; set; }

        /// <summary>
        /// Place ID to restrict search to
        /// </summary>
        private string ContainedWithin { get; set; }

        /// <summary>
        /// Name/value pair separated by "=" (i.e. "street_address=123 4th Street")
        /// </summary>
        private string Attribute { get; set; }

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
                   "Attribute"
               })
               .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<GeoType>(parameters["Type"]);

            switch (Type)
            {
                case GeoType.ID:
                    return BuildIDUrl(parameters);
                case GeoType.Reverse:
                    return BuildReverseUrl(parameters);
                case GeoType.Search:
                    return BuildSearchUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// Builds an url for search query
        /// </summary>
        /// <param name="parameters">URL parameters</param>
        /// <returns>URL for nearby places + parameters</returns>
        private Request BuildSearchUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("IP") &&
                !(parameters.ContainsKey("Latitude") &&
                  parameters.ContainsKey("Longitude")))
            {
                throw new ArgumentException("Either Latitude and Longitude or IP address is required.");
            }

            var req = new Request(BaseUrl + "geo/search.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add(new QueryParameter("lat", Latitude.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
                urlParams.Add(new QueryParameter("long", Longitude.ToString(CultureInfo.InvariantCulture)));
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

            if (parameters.ContainsKey("Attribute"))
            {
                // TODO should really be able to search for more than one Attribute
                Attribute = parameters["Attribute"] ?? String.Empty;
                var parts = Attribute.Split('=');

                if (parts.Length < 2)
                {
                    throw new ArgumentException(
                        "Attribute must be a name/value pair (i.e. street_address=123); actual value: " + Attribute,
                        "Attribute");
                }

                urlParams.Add(new QueryParameter("attribute:" + parts[0], parts[1]));
            }

            return req;
        }

        /// <summary>
        /// construct a base show url
        /// </summary>
        /// <param name="url">base show url</param>
        /// <returns>base url + show segment</returns>
        private Request BuildIDUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
                throw new ArgumentException("ID is required for a Geo ID query.", "ID");

            ID = parameters["ID"];

            var url = "geo/id/" + ID + ".json";
            return new Request(BaseUrl + url);
        }

        /// <summary>
        /// return a saved searches url
        /// </summary>
        /// <returns>saved search url</returns>
        private Request BuildReverseUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Latitude") || !parameters.ContainsKey("Longitude"))
            {
                throw new ArgumentException("Latitude and Longitude parameters are required.");
            }

            var req = new Request(BaseUrl + "geo/reverse_geocode.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add(new QueryParameter("lat", Latitude.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
                urlParams.Add(new QueryParameter("long", Longitude.ToString(CultureInfo.InvariantCulture)));
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

        /// <summary>
        /// transforms XML into IList of SavedSearch
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of SavedSearch</returns>
        public List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            List<XElement> responseItems = new List<XElement>();

            // place_type under root means that it's an ID query
            if (twitterResponse.Element("place_type") != null)
            {
                responseItems.Add(
                    new XElement("root",
                        new XElement("contained_within",
                            new XElement("item", twitterResponse.Elements()))));
            }
            else // reverse geocode query
            {
                responseItems =
                    twitterResponse
                        .Element("result")
                            .Element("places")
                                .Elements("item").ToList();
            }

            var geo =
               new Geo
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
                   Places =
                       (from pl in responseItems
                        select Place.CreatePlace(pl.Element("contained_within").Element("item")))
                        .ToList()
               };

            return new List<Geo> { geo }.OfType<T>().ToList();
        }
    }
}
