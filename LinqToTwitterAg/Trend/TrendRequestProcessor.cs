using System;
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
    public class TrendRequestProcessor<T> : IRequestProcessor<T>
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
            var url = "trends/" + parameters["WeoID"] + ".xml";

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

            var req = new Request(BaseUrl + "trends/available.xml");
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
        public virtual List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<trends></trends>";
            }

            XElement twitterResponse = XElement.Parse(responseXml);
            XNamespace itemNS = "item";

            List<XElement> locations = new List<XElement>();
            List<XElement> items = null;
            DateTime asOf = DateTime.UtcNow;
            XElement locationElement = null;
            List<Trend> trends;

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
                var trendsElement = twitterResponse.Element("trends");
                locationElement = trendsElement.Element("locations").Element("location");
                items = trendsElement.Elements("trend").ToList();
                asOf = DateTime.Parse(trendsElement.Attribute("as_of").Value,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }
            else if (twitterResponse.Element("trends") == null)
            {
                items = new List<XElement>();
            }
            else if (twitterResponse.Element("trends").Element(itemNS + "item") == null)
            {
                items = twitterResponse.Element("trends").Elements("item").ToList();
                asOf = DateTime.Parse(twitterResponse.Element("as_of").Value,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
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
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            trends =
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
                            trend.Attribute("url").Value :
                        trend.Element("url").Value
                 let name =
                    trend.Element("name") == null ?
                        trend.Value :
                        trend.Element("name").Value
                 let trendLoc =
                    Location.CreateLocation(
                        locationElement ?? trend.Element("location"))
                 let locs =
                    (from loc in locations
                     select Location.CreateLocation(loc))
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

            return trends.OfType<T>().ToList();
        }
    }
}
