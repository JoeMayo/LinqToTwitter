using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// helps process trend requests
    /// </summary>
    public class TrendRequestProcessor : IRequestProcessor
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

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<TrendType>(parameters["Type"]);

            switch (Type)
            {
                case TrendType.Current:
                    url = BuildCurrentTrendsUrl(parameters);
                    break;
                case TrendType.Daily:
                    url = BuildDailyTrendsUrl(parameters);
                    break;
                case TrendType.Trend:
                    url = BuildTrendsUrl(parameters);
                    break;
                case TrendType.Weekly:
                    url = BuildWeeklyTrendsUrl(parameters);
                    break;
                case TrendType.Available:
                    url = BuildAvailableTrendsUrl(parameters);
                    break;
                case TrendType.Location:
                    url = BuildLocationTrendsUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// Builds a url for finding trends at a specified location
        /// </summary>
        /// <param name="parameters">parameters should contain WeoID</param>
        /// <returns>base url + location segment</returns>
        private string BuildLocationTrendsUrl(Dictionary<string, string> parameters)
        {
            string url = BaseUrl;

            if (parameters.ContainsKey("WeoID"))
            {
                WeoID = int.Parse(parameters["WeoID"]);
                url += "trends/" + parameters["WeoID"] + ".xml";
            }
            else
            {
                throw new ArgumentException("WeoID is a required parameter.", "WeoID");
            }

            return url;
        }

        /// <summary>
        /// Builds an URL for finding where trends are occurring
        /// </summary>
        /// <param name="parameters">parameters can include Latitude and Longitude (must have either both parameter or neither)</param>
        /// <returns>base url + Available segment</returns>
        private string BuildAvailableTrendsUrl(Dictionary<string, string> parameters)
        {
            if ((parameters.ContainsKey("Latitude") && !parameters.ContainsKey("Longitude")) ||
                (!parameters.ContainsKey("Latitude") && parameters.ContainsKey("Longitude")))
            {
                throw new ArgumentException("If you pass either Latitude or Longitude then you must pass both. Otherwise, don't pass either.");
            }

            var url = BaseUrl + "trends/available.xml";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Latitude"))
            {
                Latitude = parameters["Latitude"];
                urlParams.Add("lat=" + parameters["Latitude"]);
            }

            if (parameters.ContainsKey("Longitude"))
            {
                Longitude = parameters["Longitude"];
                urlParams.Add("long=" + parameters["Longitude"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing daily trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildDailyTrendsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "trends/daily.json";

            url = BuildTrendsUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for showing current trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildCurrentTrendsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "trends/current.json";

            url = BuildTrendsUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for showing weekly trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildWeeklyTrendsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "trends/weekly.json";

            url = BuildTrendsUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for showing trends
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildTrendsUrl(Dictionary<string, string> parameters)
        {
            return BaseUrl + "trends.json";
        }

        /// <summary>
        /// appends parameters for trends
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildTrendsUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("Date"))
            {
                Date = DateTime.Parse(parameters["Date"]);
                urlParams.Add("date=" + DateTime.Parse(parameters["Date"], CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
            }

            if (parameters.ContainsKey("ExcludeHashtags") &&
                bool.Parse(parameters["ExcludeHashtags"]) == true)
            {
                ExcludeHashtags = bool.Parse(parameters["ExcludeHashtags"]);
                urlParams.Add("exclude=hashtags");
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        public virtual IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            XNamespace itemNS = "item";

            List<XElement> locations = new List<XElement>();
            List<XElement> items = null;
            DateTime asOf = DateTime.Now;
            XElement locationElement = null;

            if (twitterResponse.Name.LocalName == "locations")
            {
                locations = twitterResponse.Elements("location").ToList();
                items = new List<XElement>
                {
                    new XElement("item")
                };
            }
            else if (twitterResponse.Name.LocalName == "matching_trends")
            {
                locationElement = twitterResponse.Element("trends").Element("locations").Element("location");
                items = twitterResponse.Element("trends").Elements("trend").ToList();
                asOf = DateTime.Parse(twitterResponse.Element("trends").Attribute("as_of").Value, CultureInfo.InvariantCulture);
            }
            else if (twitterResponse.Element("trends").Element(itemNS + "item") == null)
            {
                items = twitterResponse.Element("trends").Elements("item").ToList();
                asOf = DateTime.Parse(twitterResponse.Element("as_of").Value, CultureInfo.InvariantCulture);
            }
            else
            {
                items = new List<XElement>();

                twitterResponse
                    .Element("trends")
                        .Element(itemNS + "item")
                            .Nodes().ToList().ForEach(node => items.Add(node as XElement));

                asOf = DateTime.Parse(
                    twitterResponse
                        .Element("trends")
                            .Element(itemNS + "item").Attribute("item").Value,
                    CultureInfo.InvariantCulture);
            }

            var location = new Location();

            var trends =
                (from trend in items
                 let query =
                    trend.Element("query") == null ?
                        trend.Attribute("query") == null ?
                            string.Empty :
                            trend.Attribute("query").Value :
                        trend.Element("query").Value 
                 let searchUrl =
                    trend.Element("url") == null ?
                        trend.Attribute("url") == null ?
                            string.Empty :
                            trend.Attribute("url").Value:
                        trend.Element("url").Value
                 let name =
                    trend.Element("name") == null ?
                        trend.Value :
                        trend.Element("name").Value
                 let trendLoc = 
                    location.CreateLocation(
                        locationElement ?? trend.Element("location"))
                 let locs =
                    (from loc in locations
                     select location.CreateLocation(loc))
                     .ToList()
                 select new Trend
                 {
                     Type = Type,
                     ExcludeHashtags = ExcludeHashtags,
                     Date = Date,
                     Name = name,
                     Query = query,
                     SearchUrl = searchUrl,
                     AsOf = asOf,
                     Latitude = Latitude,
                     Longitude = Longitude,
                     WeoID = WeoID,
                     Location = trendLoc,
                     Locations = locs
                 })
                 .ToList();

            return trends;
        }
    }
}
