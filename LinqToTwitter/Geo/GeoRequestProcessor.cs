using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Globalization;

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
                   "ID"
               })
               .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<GeoType>(parameters["Type"]);

            switch (Type)
            {
                case GeoType.ID:
                    url = BuildIDUrl(parameters);
                    break;
                case GeoType.Reverse:
                    url = BuildReverseUrl(parameters);
                    break;
                case GeoType.Nearby:
                    url = BuildNearbyPlacesUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// Builds an url for nearby places query
        /// </summary>
        /// <param name="parameters">URL parameters</param>
        /// <returns>URL for nearby places + parameters</returns>
        private string BuildNearbyPlacesUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("IP") &&
                !(parameters.ContainsKey("Latitude") &&
                 parameters.ContainsKey("Longitude")))
            {
                throw new ArgumentException("Either Latitude and Longitude or IP address is required.");
            }

            IP = parameters["IP"];

            var url = BaseUrl +"geo/nearby_places.json?ip=" + parameters["IP"];

            return url;
        }

        /// <summary>
        /// construct a base show url
        /// </summary>
        /// <param name="url">base show url</param>
        /// <returns>base url + show segment</returns>
        private string BuildIDUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("ID"))
            {
                throw new ArgumentException("ID is required for a Geo ID query.", "ID");
            }

            ID = parameters["ID"];

            var url = BaseUrl +"geo/id/" + ID + ".json";

            return url;
        }

        /// <summary>
        /// return a saved searches url
        /// </summary>
        /// <returns>saved search url</returns>
        private string BuildReverseUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("Latitude") || !parameters.ContainsKey("Longitude"))
            {
                throw new ArgumentException("Latitude and Longitude parameters are required.");
            }

            var url = BaseUrl + "geo/reverse_geocode.json";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = double.Parse(parameters["Latitude"]);
                urlParams.Add("lat=" + parameters["Latitude"]);
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = double.Parse(parameters["Longitude"]);
                urlParams.Add("long=" + parameters["Longitude"]);
            }

            if (parameters.ContainsKey("Accuracy"))
            {
                Accuracy = parameters["Accuracy"];
                urlParams.Add("accuracy=" + parameters["Accuracy"]);
            }

            if (parameters.ContainsKey("Granularity"))
            {
                Granularity = parameters["Granularity"];
                urlParams.Add("granularity=" + parameters["Granularity"]);
            }

            if (parameters.ContainsKey("MaxResults"))
            {
                MaxResults = int.Parse(parameters["MaxResults"]);
                urlParams.Add("max_results=" + parameters["MaxResults"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IList of SavedSearch
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IList of SavedSearch</returns>
        public List<T> ProcessResults(XElement twitterResponse)
        {
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

            var place = new Place();
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
                   Places =
                       (from pl in responseItems
                        select place.CreatePlace(pl.Element("contained_within").Element("item")))
                        .ToList()
               };

            return new List<Geo> { geo }.OfType<T>().ToList();
        }
    }
}
