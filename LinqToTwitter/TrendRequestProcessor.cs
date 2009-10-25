using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.Globalization;

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
        public string BaseUrl { get; set; }

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
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Trend>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Date",
                       "ExcludeHashtags"
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
                url = BuildTrendsUrl(parameters);
                return url;
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
                default:
                    url = BuildTrendsUrl(parameters);
                    break;
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

//<root type=""object"">
//  <trends type=""array"">
//    <item type=""object"">
//      <name type=""string"">Wolverine</name>
//      <url type=""string"">http://search.twitter.com/search?q=Wolverine+OR+%23Wolverine</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Swine Flu</name>
//      <url type=""string"">http://search.twitter.com/search?q=%22Swine+Flu%22</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#swineflu</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23swineflu</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Dollhouse</name>
//      <url type=""string"">http://search.twitter.com/search?q=Dollhouse+OR+%23dollhouse</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Hamthrax</name>
//      <url type=""string"">http://search.twitter.com/search?q=Hamthrax+OR+%23hamthrax</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">H1N1</name>
//      <url type=""string"">http://search.twitter.com/search?q=H1N1</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">X-Men Origins</name>
//      <url type=""string"">http://search.twitter.com/search?q=%22X-Men+Origins%22</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#outdoorplay</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23outdoorplay</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Earthquake</name>
//      <url type=""string"">http://search.twitter.com/search?q=Earthquake+OR+%23earthquake</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#jonaslive</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23jonaslive</url>
//    </item>
//  </trends>
//  <as_of type=""string"">Sat, 02 May 2009 02:38:00 +0000</as_of>
//</root>

//<root type="object">
//  <trends type="object">
//    <a:item xmlns:a="item" item="2009-05-02 03:07:50" type="array">
//      <item type="object">
//        <query type="string">Wolverine OR #wolverine</query>
//        <name type="string">Wolverine</name>
//      </item>
//      <item type="object">
//        <query type="string">"Swine Flu"</query>
//        <name type="string">Swine Flu</name>
//      </item>
//      <item type="object">
//        <query type="string">#SwineFlu</query>
//        <name type="string">#SwineFlu</name>
//      </item>
//      <item type="object">
//        <query type="string">H1N1</query>
//        <name type="string">H1N1</name>
//      </item>
//      <item type="object">
//        <query type="string">Dollhouse OR #dollhouse</query>
//        <name type="string">Dollhouse</name>
//      </item>
//      <item type="object">
//        <query type="string">Hamthrax</query>
//        <name type="string">Hamthrax</name>
//      </item>
//      <item type="object">
//        <query type="string">"X-Men Origins"</query>
//        <name type="string">X-Men Origins</name>
//      </item>
//      <item type="object">
//        <query type="string">Hawks</query>
//        <name type="string">Hawks</name>
//      </item>
//      <item type="object">
//        <query type="string">#jonaslive</query>
//        <name type="string">#jonaslive</name>
//      </item>
//      <item type="object">
//        <query type="string">#fitfam</query>
//        <name type="string">#fitfam</name>
//      </item>
//    </a:item>
//  </trends>
//  <as_of type="number">1241233670</as_of>
//</root>
        
        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        public IList ProcessResults(System.Xml.Linq.XElement twitterResponse)
        {
            XNamespace itemNS = "item";

            List<XElement> items = null;

            DateTime asOf;

            if (twitterResponse.Element("trends").Element(itemNS + "item") == null)
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

            var trends =
                (from trend in items
                 let query =
                    trend.Element("url") == null ?
                        trend.Element("query").Value :
                        trend.Element("url").Value
                 select new Trend
                 {
                     Type = Type,
                     ExcludeHashtags = ExcludeHashtags,
                     Date = Date,
                     Name = trend.Element("name").Value,
                     Query = query,
                     AsOf = asOf
                 })
                 .ToList();

            return trends;
        }
    }
}
