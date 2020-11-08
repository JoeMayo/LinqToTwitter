using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes Twitter Friendship queries
    /// </summary>
    public class FriendshipRequestProcessor<T> :
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
        public FriendshipType Type { get; set; }

        /// <summary>
        /// ID of source user
        /// </summary>
        public ulong SourceUserID { get; set; }

        /// <summary>
        /// Screen name of source user
        /// </summary>
        public string SourceScreenName { get; set; }

        /// <summary>
        /// ID of target user
        /// </summary>
        public ulong TargetUserID { get; set; }

        /// <summary>
        /// Screen name of target user
        /// </summary>
        public string TargetScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of names for lookup
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Comma-separated list of user IDs to lookup
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Helps in paging results for queries such as incoming and outgoing
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Removes status when set to true (false by default)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Removes entities on users when set to false (true by default)
        /// </summary>
        public bool IncludeUserEntities { get; set; }

        /// <summary>
        /// Number of ids to return for each request (max: 5000)
        /// </summary>
        public int Count { get; set; }

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
                       "IncludeEntities",
                       "IncludeUserEntities",
                       "Count"
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

            Type = RequestProcessorHelper.ParseEnum<FriendshipType>(parameters["Type"]);

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
                case FriendshipType.FollowerIDs:
                    return BuildFollowerIDsUrl(parameters);
                case FriendshipType.FriendIDs:
                    return BuildFriendIDsUrl(parameters);
                default:
                    throw new ArgumentException("Invalid FriendshipType", "Type");
            }
        }

        /// <summary>
        /// Builds an url that retrieves ids of people who the logged in user doesn't want retweets for
        /// </summary>
        /// <returns>no_retweet_id URL</returns>
        Request BuildFriendshipNoRetweetIDsUrl()
        {
            return new Request(BaseUrl + "friendships/no_retweets/ids.json");
        }

        /// <summary>
        /// builds an url for showing friendship details between two users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildFriendshipShowUrl(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("SourceUserID") && !parameters.ContainsKey("SourceScreenName"))
                throw new ArgumentException("You must specify either SourceUserID or SourceScreenName");

            if (!parameters.ContainsKey("TargetUserID") && !parameters.ContainsKey("TargetScreenName"))
                throw new ArgumentException("You must specify either TargetUserID or TargetScreenName");

            var req = new Request(BaseUrl + "friendships/show.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("SourceUserID"))
            {
                SourceUserID = ulong.Parse(parameters["SourceUserID"]);
                urlParams.Add(new QueryParameter("source_id", parameters["SourceUserID"]));
            }

            if (parameters.ContainsKey("SourceScreenName"))
            {
                SourceScreenName = parameters["SourceScreenName"];
                urlParams.Add(new QueryParameter("source_screen_name", SourceScreenName));
            }

            if (parameters.ContainsKey("TargetUserID"))
            {
                TargetUserID = ulong.Parse(parameters["TargetUserID"]);
                urlParams.Add(new QueryParameter("target_id", parameters["TargetUserID"]));
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
        Request BuildFriendshipIncomingUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/incoming.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        /// <summary>
        /// Build url for determining relationship between logged in user and list of other users
        /// </summary>
        /// <param name="parameters">Should contain ScreenName</param>
        /// <returns>Url for lookup</returns>
        Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/lookup.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID with a comma-separated list of twitter screen names or user IDs, respectively.");

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
        Request BuildFriendshipOutgoingUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friendships/outgoing.json");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            return req;
        }

        Request BuildFollowersListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "followers/list.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID with a comma-separated list of twitter screen names or user IDs, respectively.");

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
                        
            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", SkipStatus.ToString().ToLower()));
            }

            if (parameters.ContainsKey("IncludeEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", parameters["IncludeEntities"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeUserEntities"))
            {
                IncludeEntities = bool.Parse(parameters["IncludeUserEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", parameters["IncludeUserEntities"].ToLower()));
            }

            return req;
        }

        Request BuildFriendsListUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "friends/list.json");
            var urlParams = req.RequestParameters;

            if (!parameters.ContainsKey("ScreenName") && !parameters.ContainsKey("UserID"))
                throw new ArgumentNullException("ScreenNameOrUserID", "Requires ScreenName or UserID.");

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
            
            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }

            if (parameters.ContainsKey("SkipStatus"))
            {
                SkipStatus = bool.Parse(parameters["SkipStatus"]);
                urlParams.Add(new QueryParameter("skip_status", parameters["SkipStatus"].ToLower()));
            }

            if (parameters.ContainsKey("IncludeUserEntities"))
            {
                IncludeUserEntities = bool.Parse(parameters["IncludeUserEntities"]);
                urlParams.Add(new QueryParameter("include_user_entities", parameters["IncludeUserEntities"].ToLower()));
            }

            return req;
        }

        Request BuildFollowerIDsUrl(Dictionary<string, string> parameters)
        {
            var url = "followers/ids.json";

            return BuildIdQueryUrlParameters(parameters, url);
        }

        Request BuildFriendIDsUrl(Dictionary<string, string> parameters)
        {
            var url = "friends/ids.json";

            return BuildIdQueryUrlParameters(parameters, url);
        }

        Request BuildIdQueryUrlParameters(Dictionary<string, string> parameters, string url)
        {
            if (!parameters.ContainsKey("UserID") && !parameters.ContainsKey("ScreenName"))
                throw new ArgumentException("You must specify either UserID or ScreenName.");

            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey("UserID"))
            {
                UserID = parameters["UserID"];
                urlParams.Add(new QueryParameter("user_id", parameters["UserID"]));
            }

            if (parameters.ContainsKey("ScreenName"))
            {
                ScreenName = parameters["ScreenName"];
                urlParams.Add(new QueryParameter("screen_name", parameters["ScreenName"]));
            }

            if (parameters.ContainsKey("Cursor"))
            {
                Cursor = long.Parse(parameters["Cursor"]);
                urlParams.Add(new QueryParameter("cursor", parameters["Cursor"]));
            }
            else
            {
                Cursor = -1;
                urlParams.Add(new QueryParameter("cursor", Cursor.ToString()));
            }

            if (parameters.ContainsKey("Count"))
            {
                Count = int.Parse(parameters["Count"]);
                urlParams.Add(new QueryParameter("count", parameters["Count"]));
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
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            Friendship friendship;

            switch (Type)
            {
                case FriendshipType.Show:
                    friendship = HandleShowResponse(responseJson);
                    break;
                case FriendshipType.Incoming:
                case FriendshipType.Outgoing:
                case FriendshipType.FollowerIDs:
                case FriendshipType.FriendIDs:
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
            friendship.IncludeUserEntities = IncludeUserEntities;
            friendship.Count = Count;

            var friendList = new List<Friendship>
            {
                friendship
            };

            return friendList.OfType<T>().ToList();
        }
  
        Friendship HandleShowResponse(string responseJson)
        {
            JsonElement showJson = JsonDocument.Parse(responseJson).RootElement;
            var friendship = new Friendship(showJson.GetProperty("relationship"));
            return friendship;
        }
  
        Friendship HandleIdsResponse(string responseJson)
        {
            JsonElement idsJson = JsonDocument.Parse(responseJson).RootElement;
            var friendship = new Friendship
            {
                IDInfo = new IDList(idsJson),
                CursorMovement = new Cursors(idsJson)
            };
            return friendship;
        }
  
        Friendship HandleLookupResponse(string responseJson)
        {
            JsonElement lookupJson = JsonDocument.Parse(responseJson).RootElement;
            var friendship = new Friendship
            {
                Relationships =
                    (from relationship in lookupJson.EnumerateArray()
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

        Friendship HandleFriendsListOrFollowersListResponse(string responseJson)
        {
            JsonElement friendsOrFollowersJson = JsonDocument.Parse(responseJson).RootElement;
            var users = friendsOrFollowersJson.GetProperty("users");

            var friendship = new Friendship
            {
                CursorMovement = new Cursors(friendsOrFollowersJson),
                Users =
                    (from user in users.EnumerateArray()
                     select new User(user))
                    .ToList()
            };
            return friendship;
        }

        public T ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement friendJson = JsonDocument.Parse(responseJson).RootElement;

            switch ((FriendshipAction) theAction)
            {
                case FriendshipAction.Create:
                case FriendshipAction.Destroy:
                    var user = new User(friendJson);
                    return user.ItemCast(default(T));
                case FriendshipAction.Update:
                    var friendship = new Friendship(friendJson.GetProperty("relationship"));
                    return friendship.ItemCast(default(T));
                default:
                    throw new InvalidOperationException("Unknown Action.");
            }
        }
    }
}
