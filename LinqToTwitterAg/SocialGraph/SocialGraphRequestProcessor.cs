using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Processes Social Graph Requests and responses
    /// </summary>
    class SocialGraphRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// type of request
        /// </summary>
        private SocialGraphType Type { get; set; }

        /// <summary>
        /// The ID or screen_name of the user to retrieve the friends ID list for
        /// </summary>
        private string ID { get; set; }

        /// <summary>
        /// Specfies the ID of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid user ID is also a valid screen name. 
        /// </summary>
        private ulong UserID { get; set; }

        /// <summary>
        /// Specfies the screen name of the user for whom to return the friends list. 
        /// Helpful for disambiguating when a valid screen name is also a user ID.
        /// </summary>
        private string ScreenName { get; set; }

        /// <summary>
        /// Page to return (obsolete, use Cursor instead)
        /// </summary>
        [Obsolete(
            "This property has been deprecated and will be ignored by Twitter. Please use Cursor/CursorResponse instead.",
            /*error:*/ true)]
        private int Page { get; set; }

        /// <summary>
        /// Indicator for which page to get next
        /// </summary>
        /// <remarks>
        /// This is not a page number, but is an indicator to
        /// Twitter on which page you need back. Your choices
        /// are Previous and Next, which you can find in the
        /// CursorResponse property when your response comes back.
        /// </remarks>
        private string Cursor { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<SocialGraph>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "ID",
                       "UserID",
                       "ScreenName",
                       "Page",
                       "Cursor"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
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

            Type = RequestProcessorHelper.ParseQueryEnumType<SocialGraphType>(parameters["Type"]);

            switch (Type)
            {
                case SocialGraphType.Followers:
                    return BuildSocialGraphFollowersUrl(parameters);
                case SocialGraphType.Friends:
                    return BuildSocialGraphFriendsUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSocialGraphFriendsUrl(Dictionary<string, string> parameters)
        {
            var url = "friends/ids.xml";

            return BuildSocialGraphUrlParameters(parameters, url);
        }

        /// <summary>
        /// builds an url for showing status of user
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildSocialGraphFollowersUrl(Dictionary<string, string> parameters)
        {
            var url = "followers/ids.xml";

            return BuildSocialGraphUrlParameters(parameters, url);
        }

        /// <summary>
        /// appends parameters for Friendship action
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        private Request BuildSocialGraphUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (!parameters.ContainsKey("ID") && !parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
                throw new ArgumentException("You must specify either ID, UserID, or ScreenName.");

            if (parameters.ContainsKey("ID"))
            {
                ID = parameters["ID"];
                url = BuildUrlHelper.TransformIDUrl(parameters, url);
            }

            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = ulong.Parse(parameters["UserID"]);
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }
            else
            {
                Cursor = "-1";
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            return req;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<ids></ids>";
            }

            XElement twitterResponse = XElement.Parse(responseXml);
            var graph = new SocialGraph
            {
                Type = Type,
                ID = ID,
                UserID = UserID,
                ScreenName = ScreenName,
                Cursor = Cursor,
                CursorMovement = new Cursors
                {
                    Next =
                        twitterResponse.Element("next_cursor") == null ?
                            string.Empty :
                            twitterResponse.Element("next_cursor").Value,
                    Previous =
                        twitterResponse.Element("previous_cursor") == null ?
                            string.Empty :
                            twitterResponse.Element("previous_cursor").Value
                }
            };

            IEnumerable<string> idList = null;

            // TODO: analyze to determine if this (CursorMovement and IDs) can be refactored to use IDList list as done in friendship/incoming and friendship/outgoing. 
            //  Would be a breaking change, but yet pull API into consistent usage in this area. 
            //  Because of the if statement this might not be straight forward, but then again, if statement might be OBE since initial API creation and all that is needed is to parse IDs rather than a single ID. - Joe 4/16/2010

            // we get back ids if using cursors but id if not using cursors
            if (twitterResponse.Element("ids") == null)
            {
                idList =
                    from id in twitterResponse.Elements("id").ToList()
                    select id.Value;
            }
            else
            {
                idList =
                    from id in twitterResponse.Element("ids").Elements("id").ToList()
                    select id.Value;
            }

            graph.IDs = idList.ToList();

            return new List<SocialGraph> { graph }.OfType<T>().ToList();
        }
    }
}
