using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Status requests
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
                       "ID",
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

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                url = BuildPublicUrl();
                return url;
            }

            switch ((StatusType)Enum.ToObject(typeof(StatusType), int.Parse(parameters["Type"])))
            {
                case StatusType.Public:
                    url = BuildPublicUrl();
                    break;
                case StatusType.Friends:
                    url = BuildFriendUrl(parameters);
                    break;
                case StatusType.User:
                    url = BuildUserUrl(parameters);
                    break;
                case StatusType.Show:
                    url = BuildShowUrl(parameters);
                    break;
                case StatusType.Replies:
                    url = BuildRepliesUrl(parameters);
                    break;
                default:
                    url = BuildPublicUrl();
                    break;
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildRepliesUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/replies.xml";

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildShowUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/show.xml";

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            return url;
        }

        /// <summary>
        /// appends parameters that are common to both friend and user queries
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private string BuildFriendRepliesAndUrlParameters(Dictionary<string, string> parameters, string url)
        {
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
        /// construct a base user url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + user timeline segment</returns>
        private string BuildUserUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/user_timeline.xml";

            url = BuildUrlHelper.TransformIDUrl(parameters, url);

            url = BuildFriendRepliesAndUrlParameters(parameters, url);

            return url;
        }

        /// <summary>
        /// construct a base friend url
        /// </summary>
        /// <param name="url">base status url</param>
        /// <returns>base url + friend segment</returns>
        private string BuildFriendUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "statuses/friends_timeline.xml";
            
            url = BuildFriendRepliesAndUrlParameters(parameters, url);
            
            return url;
        }

        /// <summary>
        /// return a public url
        /// </summary>
        /// 
        /// <returns>base url + public segment</returns>
        private string BuildPublicUrl()
        {
            var url = BaseUrl + "statuses/public_timeline.xml";
            return url;
        }

//<statuses type="array">
//  <status>
//    <created_at>Wed Oct 15 19:41:49 +0000 2008</created_at>
//    <id>961102353</id>
//    <text>My latest blog post: Where in the world is Blake Stone? http://tinyurl.com/4bd9hv</text>
//    <source>web</source>
//    <truncated>false</truncated>
//    <in_reply_to_status_id></in_reply_to_status_id>
//    <in_reply_to_user_id></in_reply_to_user_id>
//    <favorited>false</favorited>
//    <in_reply_to_screen_name></in_reply_to_screen_name>
//    <user>
//      <id>15411837</id>
//      <name>Joe Mayo</name>
//      <screen_name>JoeMayo</screen_name>
//      <location>Denver, CO</location>
//      <description>Author/entrepreneur, specializing in custom .NET software development</description>
//      <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
//      <url>http://www.csharp-station.com</url>
//      <protected>false</protected>
//      <followers_count>25</followers_count>
//    </user>
//  </status>        

        /// <summary>
        /// transforms XML into IQueryable of Status
        /// </summary>
        /// <param name="twitterResponse">xml with Twitter response</param>
        /// <returns>IQueryable of Status</returns>
        public IQueryable ProcessResults(XElement twitterResponse)
        {
            var responseItems = twitterResponse.Elements("status").ToList();

            // if we get only a single response back,
            // such as a Show request, make sure we get it
            if (twitterResponse.Name == "status")
            {
                responseItems.Add(twitterResponse);
            }

            var statusList =
                from status in responseItems
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
                       InReplyToScreenName = 
                        status.Element("in_reply_to_screen_name") == null ?
                            string.Empty :
                            status.Element("in_reply_to_screen_name").Value,
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
