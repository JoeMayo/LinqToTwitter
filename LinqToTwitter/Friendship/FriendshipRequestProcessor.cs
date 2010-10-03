using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Friendship queries
    /// </summary>
    class FriendshipRequestProcessor<T> : IRequestProcessor<T>
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of friendship (defaults to Exists)
        /// </summary>
        private FriendshipType Type { get; set; }

        /// <summary>
        /// The ID or screen_name of the subject user
        /// </summary>
        private string SubjectUser { get; set; }

        /// <summary>
        /// The ID or screen_name of the user to test for following
        /// </summary>
        private string FollowingUser { get; set; }

        /// <summary>
        /// ID of source user
        /// </summary>
        private string SourceUserID { get; set; }

        /// <summary>
        /// Screen name of source user
        /// </summary>
        private string SourceScreenName { get; set; }

        /// <summary>
        /// ID of target user
        /// </summary>
        private string TargetUserID { get; set; }

        /// <summary>
        /// Screen name of target user
        /// </summary>
        private string TargetScreenName { get; set; }


        /// <summary>
        /// Helps in paging results for queries such as incoming and outgoing
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Friendship>(
                   lambdaExpression.Body,
                   new List<string> { 
                       "Type",
                       "SubjectUser",
                       "FollowingUser",
                       "SourceUserID",
                       "SourceScreenName",
                       "TargetUserID",
                       "TargetScreenName",
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
        public virtual string BuildURL(Dictionary<string, string> parameters)
        {
            string url = null;

            if (parameters == null || !parameters.ContainsKey("Type"))
            {
                throw new ArgumentException("You must set Type.", "Type");
            }

            Type = RequestProcessorHelper.ParseQueryEnumType<FriendshipType>(parameters["Type"]);

            switch (Type)
            {
                case FriendshipType.Exists:
                    url = BuildFriendshipExistsUrl(parameters);
                    break;
                case FriendshipType.Show:
                    url = BuildFriendshipShowUrl(parameters);
                    break;
                case FriendshipType.Incoming:
                    url = BuildFriendshipIncomingUrl(parameters);
                    break;
                case FriendshipType.Outgoing:
                    url = BuildFriendshipOutgoingUrl(parameters);
                    break;
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

            return url;
        }

        /// <summary>
        /// builds an url for determining if friendship exists between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + exists segment</returns>
        private string BuildFriendshipExistsUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "friendships/exists.xml";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("SubjectUser"))
            {
                SubjectUser = parameters["SubjectUser"];
                urlParams.Add("user_a=" + parameters["SubjectUser"]);
            }
            else
            {
                throw new ArgumentException("SubjectUser is required.", "SubjectUser");
            }

            if (parameters.ContainsKey("FollowingUser"))
            {
                FollowingUser = parameters["FollowingUser"];
                urlParams.Add("user_b=" + parameters["FollowingUser"]);
            }
            else
            {
                throw new ArgumentException("FollowingUser is required.", "FollowingUser");
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// builds an url for showing friendship details between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private string BuildFriendshipShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("SourceUserID") && !parameters.ContainsKey("SourceScreenName"))
            {
                throw new ArgumentException("You must specify either SourceUserID or SourceScreenName");
            }

            if (!parameters.ContainsKey("TargetUserID") && !parameters.ContainsKey("TargetScreenName"))
            {
                throw new ArgumentException("You must specify either TargetUserID or TargetScreenName");
            }

            var url = BaseUrl + "friendships/show.xml";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("SourceUserID"))
            {
                SourceUserID = parameters["SourceUserID"];
                urlParams.Add("source_id=" + parameters["SourceUserID"]);
            }

            if (parameters.ContainsKey("SourceScreenName"))
            {
                SourceScreenName = parameters["SourceScreenName"];
                urlParams.Add("source_screen_name=" + parameters["SourceScreenName"]);
            }

            if (parameters.ContainsKey("TargetUserID"))
            {
                TargetUserID = parameters["TargetUserID"];
                urlParams.Add("target_id=" + parameters["TargetUserID"]);
            }

            if (parameters.ContainsKey("TargetScreenName"))
            {
                TargetScreenName = parameters["TargetScreenName"];
                urlParams.Add("target_screen_name=" + parameters["TargetScreenName"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// Build url for determining incoming friend requests
        /// </summary>
        /// <param name="parameters">Can optionally contain Cursor</param>
        /// <returns>Url for incoming</returns>
        private string BuildFriendshipIncomingUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "friendships/incoming.xml";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add("cursor=" + parameters["Cursor"]);
            }

            if (urlParams.Count > 0)
            {
                url += "?" + string.Join("&", urlParams.ToArray());
            }

            return url;
        }

        /// <summary>
        /// Build url for determining outgoing friend requests
        /// </summary>
        /// <param name="parameters">Can optionally contain Cursor</param>
        /// <returns>Url for outgoing</returns>
        private string BuildFriendshipOutgoingUrl(Dictionary<string, string> parameters)
        {
            var url = BaseUrl + "friendships/outgoing.xml";

            var urlParams = new List<string>();

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add("cursor=" + parameters["Cursor"]);
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
        /// <returns>IQueryable of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            XElement twitterResponse = XElement.Parse(responseXml);
            var friendship =
                new Friendship
                {
                    Type = Type,
                    SubjectUser = SubjectUser,
                    FollowingUser = FollowingUser,
                    SourceUserID = SourceUserID,
                    SourceScreenName = SourceScreenName,
                    TargetUserID = TargetUserID,
                    TargetScreenName = TargetScreenName,
                    Cursor = Cursor
                };

            if (twitterResponse.Name == "relationship") // Show
            {
                var relationship = new Relationship();

                friendship.SourceRelationship =
                    relationship.CreateRelationship(twitterResponse.Element("source"));
                friendship.TargetRelationship =
                    relationship.CreateRelationship(twitterResponse.Element("target"));
            }
            else if (twitterResponse.Name == "id_list") // incoming/outgoing
            {
                friendship.IDInfo = new IDList().CreateIDList(twitterResponse);
            }
            else // Exists
            {
                friendship.IsFriend = bool.Parse(twitterResponse.Value);
            }

            var friendList = new List<Friendship>
            {
                friendship
            };

            return friendList.OfType<T>().ToList();
        }
    }
}
