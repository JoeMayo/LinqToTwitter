using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Collections;
using System.Globalization;

namespace LinqToTwitter
{
    /// <summary>
    /// Manages request processing for favorites
    /// </summary>
    public class FavoritesRequestProcessor<T> : IRequestProcessor<T>
    {
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of favorites to query
        /// </summary>
        private FavoritesType Type { get; set; }

        /// <summary>
        /// User identity to search (optional)
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        /// Page to retrieve (optional)
        /// </summary>
        private int Page { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<Favorites>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Page",
                       "ID"
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
            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<FavoritesType>(parameters["Type"]);

            var url = BaseUrl + "favorites.xml";

            url = BuildFavoritesUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters for Favorites request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildFavoritesUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var urlParams = new List<string>();

            if (parameters.ContainsKey("Page"))
            {
                Page = int.Parse(parameters["Page"]);
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                urlParams.Add("id=" + parameters["ID"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            var responseItems = twitterResponse.Elements("status").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "status")
            {
                responseItems.Add(twitterResponse);
            }

            var statusList =
                from status in responseItems
                let createdAtDate =
                    DateTime.ParseExact(
                        status.Element("created_at").Value,
                        "ddd MMM dd HH:mm:ss %zzzz yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
                let user = status.Element("user")
                select
                   new Favorites
                   {
                       Type = Type,
                       Page = Page,
                       CreatedAt = createdAtDate,
                       Favorited =
                        bool.Parse(
                            string.IsNullOrEmpty(status.Element("favorited").Value) ?
                            "true" :
                            status.Element("favorited").Value),
                       ID = status.Element("id").Value,
                       InReplyToStatusID = status.Element("in_reply_to_status_id").Value,
                       InReplyToUserID = status.Element("in_reply_to_user_id").Value,
                       Source = status.Element("source").Value,
                       Text = status.Element("text").Value,
                       Truncated = bool.Parse(status.Element("truncated").Value),
                       InReplyToScreenName =
                        status.Element("in_reply_to_screen_name") == null ?
                            string.Empty :
                            status.Element("in_reply_to_screen_name").Value,
                       User = User.CreateUser(user)
                   };

            return statusList.OfType<T>().ToList();
        }
    }
}
