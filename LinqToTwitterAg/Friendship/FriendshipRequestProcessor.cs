using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// List of names for lookup
        /// </summary>
        private string ScreenName { get; set; }

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
                       "Cursor",
                       "ScreenName"
                   });

            var parameters = paramFinder.Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildURL(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", "Type");

            Type = RequestProcessorHelper.ParseQueryEnumType<FriendshipType>(parameters["Type"]);

            switch (Type)
            {
                case FriendshipType.Exists:
                    return BuildFriendshipExistsUrl(parameters);
                case FriendshipType.Incoming:
                    return BuildFriendshipIncomingUrl(parameters);
                case FriendshipType.Lookup:
                    return BuildLookupUrl(parameters);
                case FriendshipType.Outgoing:
                    return BuildFriendshipOutgoingUrl(parameters);
                case FriendshipType.Show:
                    return BuildFriendshipShowUrl(parameters);
                case FriendshipType.NoRetweetIDs:
                    return BuildFriendshipNoRetweetIDsUrl();
                default:
                    throw new ArgumentException("Invalid FriendshipType", "Type");
            }
        }

        /// <summary>
        /// Builds an url that retrieves ids of people who the logged in user doesn't want retweets for
        /// </summary>
        /// <returns>no_retweet_id URL</returns>
        private Request BuildFriendshipNoRetweetIDsUrl()
        {
            return new Request(BaseUrl + "friendships/no_retweet_ids.xml");
        }

        /// <summary>
        /// builds an url for determining if friendship exists between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + exists segment</returns>
        private Request BuildFriendshipExistsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/exists.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("SubjectUser"))
            {
                SubjectUser = parameters["SubjectUser"];
                urlParams.Add(new QueryParameter("user_a", SubjectUser));
            }
            else
            {
                throw new ArgumentException("SubjectUser is required.", "SubjectUser");
            }

            if (parameters.ContainsKey("FollowingUser"))
            {
                FollowingUser = parameters["FollowingUser"];
                urlParams.Add(new QueryParameter("user_b", FollowingUser));
            }
            else
            {
                throw new ArgumentException("FollowingUser is required.", "FollowingUser");
            }

            return req;
        }

        /// <summary>
        /// builds an url for showing friendship details between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        private Request BuildFriendshipShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("SourceUserID") && !parameters.ContainsKey("SourceScreenName"))
            {
                throw new ArgumentException("You must specify either SourceUserID or SourceScreenName");
            }

            if (!parameters.ContainsKey("TargetUserID") && !parameters.ContainsKey("TargetScreenName"))
            {
                throw new ArgumentException("You must specify either TargetUserID or TargetScreenName");
            }

            var req = new Request(BaseUrl + "friendships/show.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("SourceUserID"))
            {
                SourceUserID = parameters["SourceUserID"];
                urlParams.Add(new QueryParameter("source_id", SourceUserID));
            }

            if (parameters.ContainsKey("SourceScreenName"))
            {
                SourceScreenName = parameters["SourceScreenName"];
                urlParams.Add(new QueryParameter("source_screen_name", SourceScreenName));
            }

            if (parameters.ContainsKey("TargetUserID"))
            {
                TargetUserID = parameters["TargetUserID"];
                urlParams.Add(new QueryParameter("target_id", TargetUserID));
            }

            if (parameters.ContainsKey("TargetScreenName"))
            {
                TargetScreenName = parameters["TargetScreenName"];
                urlParams.Add(new QueryParameter("target_screen_name", TargetScreenName));
            }

            return req;
        }

        /// <summary>
        /// Build url for determining incoming friend requests
        /// </summary>
        /// <param name="parameters">Can optionally contain Cursor</param>
        /// <returns>Url for incoming</returns>
        private Request BuildFriendshipIncomingUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/incoming.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            return req;
        }

        /// <summary>
        /// Build url for determining relationship between logged in user and list of other users
        /// </summary>
        /// <param name="parameters">Should contain ScreenName</param>
        /// <returns>Url for lookup</returns>
        private Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/lookup.xml");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName"))
            {
                throw new ArgumentNullException("ScreenName", "Requires ScreenName with a comma-separated list of twitter screen names");
            }

            ScreenName = parameters["ScreenName"];
            urlParams.Add(new QueryParameter("screen_name", ScreenName));

            return req;
        }

        /// <summary>
        /// Build url for determining outgoing friend requests
        /// </summary>
        /// <param name="parameters">Can optionally contain Cursor</param>
        /// <returns>Url for outgoing</returns>
        private Request BuildFriendshipOutgoingUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/outgoing.xml");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            return req;
        }

        /// <summary>
        /// transforms XML into IQueryable of User
        /// </summary>
        /// <param name="responseXml">xml with Twitter response</param>
        /// <returns>IQueryable of User</returns>
        public virtual List<T> ProcessResults(string responseXml)
        {
            if (string.IsNullOrEmpty(responseXml))
            {
                responseXml = "<friendship></friendship>";
            }

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
                    Cursor = Cursor,
                    ScreenName = ScreenName
                };

            if (twitterResponse.Name == "friendship")
            {
                friendship = new Friendship();
            }
            else if (twitterResponse.Name == "relationship") // Show
            {
                friendship.SourceRelationship =
                    Relationship.CreateRelationship(twitterResponse.Element("source"));
                friendship.TargetRelationship =
                    Relationship.CreateRelationship(twitterResponse.Element("target"));
            }
            else if (twitterResponse.Name == "relationships")
            {
                friendship.Relationships =
                    (from relElem in twitterResponse.Elements("relationship")
                     select Relationship.CreateRelationship(relElem))
                    .ToList();
            }
            else if (twitterResponse.Name == "id_list") // incoming/outgoing
            {
                friendship.IDInfo = IDList.CreateIDList(twitterResponse);
            }
            else if (twitterResponse.Name == "ids")
            {
                friendship.IDInfo = IDList.CreateIDs(twitterResponse);
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
