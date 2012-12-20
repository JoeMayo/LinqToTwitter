using System;
using System.Collections.Generic;
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
        /// ID of source user
        /// </summary>
        internal string SourceUserID { get; set; }

        /// <summary>
        /// Screen name of source user
        /// </summary>
        internal string SourceScreenName { get; set; }

        /// <summary>
        /// ID of target user
        /// </summary>
        internal string TargetUserID { get; set; }

        /// <summary>
        /// Screen name of target user
        /// </summary>
        internal string TargetScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of names for lookup
        /// </summary>
        internal string ScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of user IDs to lookup
        /// </summary>
        internal string UserID { get; set; }

        /// <summary>
        /// Helps in paging results for queries such as incoming and outgoing
        /// </summary>
        internal string Cursor { get; set; }

        /// <summary>
        /// Removes status when set to true (false by default)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

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
                       "SourceUserID",
                       "SourceScreenName",
                       "TargetUserID",
                       "TargetScreenName",
                       "Cursor",
                       "ScreenName",
                       "UserID",
                       "SkipStatus",
                       "IncludeEntities"
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
                case FriendshipType.FollowersList:
                    return BuildFollowersListUrl(parameters);
                case FriendshipType.FriendsList:
                    return BuildFriendsListUrl(parameters);
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

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
            {
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID with a comma-separated list of twitter screen names or user IDs, respectively.");
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", ScreenName));
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", UserID));
            }

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

        Request BuildFollowersListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "followers/list.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
            {
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID with a comma-separated list of twitter screen names or user IDs, respectively.");
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", UserID));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", ScreenName));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", SkipStatus.ToString().ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", IncludeEntities.ToString().ToLower()));
            }

            return req;
        }

        Request BuildFriendsListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friends/list.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
            {
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID with a comma-separated list of twitter screen names or user IDs, respectively.");
            }

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", UserID));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", ScreenName));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = parameters["Cursor"];
                urlParams.Add(new QueryParameter("cursor", Cursor));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", SkipStatus.ToString().ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", IncludeEntities.ToString().ToLower()));
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
                case FriendshipType.FriendsList:
                case FriendshipType.FollowersList:
                    friendship = HandleFriendsListOrFollowersListResponse(responseJson);
                    break;
                default:
                    friendship = new Friendship();
                    break;
            }

            friendship.Type = Type;
            friendship.SourceUserID = SourceUserID;
            friendship.SourceScreenName = SourceScreenName;
            friendship.TargetUserID = TargetUserID;
            friendship.TargetScreenName = TargetScreenName;
            friendship.Cursor = Cursor;
            friendship.ScreenName = ScreenName;
            friendship.UserID = UserID;
            friendship.SkipStatus = SkipStatus;
            friendship.IncludeEntities = IncludeEntities;

            var friendList = new List<Friendship>
            {
                friendship
            };

            return friendList.OfType<T>().ToList();
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

        private Friendship HandleFriendsListOrFollowersListResponse(string responseJson)
        {
            JsonData friendsOrFollowersJson = JsonMapper.ToObject(responseJson);
            var users = friendsOrFollowersJson.GetValue<JsonData>("users");

            var friendship = new Friendship
            {
                CursorMovement = new Cursors(friendsOrFollowersJson),
                Users =
                    (from JsonData user in users
                     select new User(user))
                    .ToList()
            };
            return friendship;
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
