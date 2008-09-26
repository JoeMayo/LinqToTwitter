using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    /// <summary>
    /// builds URL's for Twitter Status requests
    /// </summary>
    public class StatusRequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Status>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "Since",
                       "SinceID",
                       "Count",
                       "Page"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null ||
                !parameters.ContainsKey("Type"))
            {
                url = BaseUrl + "statuses/public_timeline.xml";
                return url;
            }

            switch (parameters["Type"])
            {
                case "Public":
                    url = BaseUrl + "statuses/public_timeline.xml";
                    break;
                case "Friends":
                    url = BaseUrl + "statuses/friends_timeline.xml";
                    break;
                default:
                    url = BaseUrl + "statuses/public_timeline.xml";
                    break;
            }

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Since"))
            {
                var sinceDateLocal = DateTime.Parse(parameters["Since"]);
                var sinceDateUtc = new DateTimeOffset(sinceDateLocal, 
                            TimeZoneInfo.Local.GetUtcOffset(sinceDateLocal));

                urlParams.Add("since=" + sinceDateUtc.ToUniversalTime().ToString("r"));
            }

            if (parameters.ContainsKey("SinceID"))
            {
                urlParams.Add("since_id=" + parameters["SinceID"]);
            }

            if (parameters.ContainsKey("Count"))
            {
                urlParams.Add("count=" + parameters["Count"]);
            }

            if (parameters.ContainsKey("Page"))
            {
                urlParams.Add("page=" + parameters["Page"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// transforms XML into IQueryable of Status
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of Status</returns>
        public IQueryable ProcessResults(XElement twitterResponse)
        {
            var statusList =
                from status in twitterResponse.Elements("status")
                let dateParts =
                    status.Element("created_at").Value.Split(' ')
                let createdAtDate =
                    DateTime.Parse(
                        string.Format("{0} {1} {2} {3} GMT",
                        dateParts[1],
                        dateParts[2],
                        dateParts[5],
                        dateParts[3]))
                let user = status.Element("user")
                select
                   new Status
                   {
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
                       User =
                           new User
                           {
                               Description = user.Element("description").Value,
                               FollowersCount = int.Parse(user.Element("followers_count").Value),
                               ID = user.Element("id").Value,
                               Location = user.Element("location").Value,
                               Name = user.Element("name").Value,
                               ProfileImageUrl = user.Element("profile_image_url").Value,
                               Protected = bool.Parse(user.Element("protected").Value),
                               ScreenName = user.Element("screen_name").Value,
                               URL = user.Element("url").Value
                           }
                   };

            var queryableStatus = statusList.AsQueryable<Status>();
            return queryableStatus;
        }
    }
}
