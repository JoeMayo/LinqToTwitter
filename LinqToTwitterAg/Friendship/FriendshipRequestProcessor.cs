using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Friendship queries
    /// </summary>
    class FriendshipRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// type of friendship (defaults to Exists)
        /// </summary>
        internal FriendshipType Type { get; set; }

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
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

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
            return new Request(BaseUrl + "friendships/no_retweet_ids.json");
        }

        /// <summary>
        /// builds an url for determining if friendship exists between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + exists segment</returns>
        private Request BuildFriendshipExistsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/exists.json");
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

            var req = new Request(BaseUrl + "friendships/show.json");
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
            var req = new Request(BaseUrl + "friendships/incoming.json");
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
            var req = new Request(BaseUrl + "friendships/lookup.json");
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
            var req = new Request(BaseUrl + "friendships/outgoing.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            return req;
        }

        /// <summary>
        /// transforms Twitter response into List of User
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrEmpty(responseJson)) return new List<T>();

            Friendship friendship;

            switch (Type)
            {
                case FriendshipType.Exists:
                    friendship = HandleExistsResponse(responseJson);
                    break;
                case FriendshipType.Show:
                    friendship = HandleShowResponse(responseJson);
                    break;
                case FriendshipType.Incoming:
                case FriendshipType.Outgoing:
                    friendship = HandleIdsResponse(responseJson);
                    break;
                case FriendshipType.Lookup:
                    friendship = HandleLookupResponse(responseJson);
                    break;
                case FriendshipType.NoRetweetIDs:
                    friendship = HandleNoRetweetIDsResponse(responseJson);
                    break;
                default:
                    friendship = new Friendship();
                    break;
            }

            friendship.Type = Type;
            friendship.SubjectUser = SubjectUser;
            friendship.FollowingUser = FollowingUser;
            friendship.SourceUserID = SourceUserID;
            friendship.SourceScreenName = SourceScreenName;
            friendship.TargetUserID = TargetUserID;
            friendship.TargetScreenName = TargetScreenName;
            friendship.Cursor = Cursor;
            friendship.ScreenName = ScreenName;

            var friendList = new List<Friendship>
            {
                friendship
            };

            return friendList.OfType<T>().ToList();
        }
  
        Friendship HandleExistsResponse(string responseJson)
        {
            bool exists;
            bool.TryParse(responseJson, out exists);
            var friendship = new Friendship { IsFriend = exists };
            return friendship;
        }
  
        Friendship HandleShowResponse(string responseJson)
        {
            JsonData showJson = JsonMapper.ToObject(responseJson);
            var friendship = new Friendship(showJson.GetValue<JsonData>("relationship"));
            return friendship;
        }
  
        Friendship HandleIdsResponse(string responseJson)
        {
            JsonData idsJson = JsonMapper.ToObject(responseJson);
            var friendship = new Friendship
            {
                IDInfo = new IDList(idsJson)
            };
            return friendship;
        }
  
        Friendship HandleLookupResponse(string responseJson)
        {
            JsonData lookupJson = JsonMapper.ToObject(responseJson);
            var friendship = new Friendship
            {
                Relationships =
                    (from JsonData relationship in lookupJson
                     select new Relationship(relationship))
                    .ToList()
            };
            return friendship;
        }

        Friendship HandleNoRetweetIDsResponse(string responseJson)
        {
            string idsJson = "{ \"ids\":" + responseJson + " }";
            return HandleIdsResponse(idsJson);
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonData friendJson = JsonMapper.ToObject(responseJson);

            switch ((FriendshipAction) theAction)
            {
                case FriendshipAction.Create:
                case FriendshipAction.Destroy:
                    var user = new User(friendJson);
                    return user.ItemCast(default(T));
                case FriendshipAction.Update:
                    var friendship = new Friendship(friendJson.GetValue<JsonData>("relationship"));
                    return friendship.ItemCast(default(T));
                default:
                    throw new InvalidOperationException("Unknown Action.");
            }
        }
    }
}
