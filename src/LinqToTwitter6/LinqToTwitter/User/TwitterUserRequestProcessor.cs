using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes user queries
    /// </summary>
    public class TwitterUserRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of query
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// User ID for following/follower queries
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        /// Required for ID queries - Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// ID for queries that need users associated with a list
        /// </summary>
        public string? ListID { get; set; }

        /// <summary>
        /// Max number of tweets to return per requrest - default 100 - possible 1000
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the media object - <see cref="MediaField"/>
        /// </summary>
        public string? MediaFields { get; set; }

        /// <summary>
        /// If set, with token from previous response metadata, pages forward or backward
        /// </summary>
        public string? PaginationToken { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the place object - <see cref="PlaceField"/>
        /// </summary>
        public string? PlaceFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the poll object - <see cref="PollField"/>
        /// </summary>
        public string? PollFields { get; set; }

        /// <summary>
        /// ID of space to query for users
        /// </summary>
        public string? SpaceID { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object - <see cref="TweetField"/>
        /// </summary>
        public string? TweetFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// Required for username queries - Up to 100 comma-separated usernames to search for
        /// </summary>
        public string? Usernames { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<TwitterUserQuery>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(ID),
                       nameof(Ids),
                       nameof(Expansions),
                       nameof(ListID),
                       nameof(MaxResults),
                       nameof(MediaFields),
                       nameof(PaginationToken),
                       nameof(PlaceFields),
                       nameof(PollFields),
                       nameof(SpaceID),
                       nameof(TweetFields),
                       nameof(UserFields),
                       nameof(Usernames),
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(Type)))
                Type = RequestProcessorHelper.ParseEnum<UserType>(parameters["Type"]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            return Type switch
            {
                UserType.IdLookup => BuildIdLookupUrl(parameters),
                UserType.Followers => BuildFollowersUrl(parameters),
                UserType.Following => BuildFollowingUrl(parameters),
                UserType.ListFollowers => BuildListFollowersUrl(parameters),
                UserType.ListMembers => BuildListMembersUrl(parameters),
                UserType.RetweetedBy => BuildRetweetedByUrl(parameters),
                UserType.SpaceBuyers => BuildSpaceBuyersUrl(parameters),
                UserType.UsernameLookup => BuildUsernameLookupUrl(parameters),
                _ => throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified."),
            };
        }

        /// <summary>
        /// builds a url to search for user info by id(s)
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        Request BuildIdLookupUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Ids)))
            {
                Ids = parameters[nameof(Ids)];
                urlParams.Add(new QueryParameter("ids", Ids.Replace(" ", "")));
            }
            else
            {
                throw new ArgumentNullException(nameof(Ids), $"{nameof(Ids)} is required!");
            }

            BuildSharedUrlParameters(urlParams, parameters);

            return req;
        }

        /// <summary>
        /// builds a url for people following user
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        Request BuildFollowersUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(ID), val => ID = val);

            var req = new Request($"{BaseUrl}users/{ID}/followers");

            BuildFollowParameters(parameters, req);

            return req;
        }

        /// <summary>
        /// builds a url for people user follows
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        Request BuildFollowingUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(ID), val => ID = val);

            var req = new Request($"{BaseUrl}users/{ID}/following");

            BuildFollowParameters(parameters, req);

            return req;
        }

        /// <summary>
        /// ListFollowers URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildListFollowersUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(ListID), val => ListID = val);

            var req = new Request($"{BaseUrl}lists/{ListID}/followers");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// ListMembers URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildListMembersUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(ListID), val => ListID = val);

            var req = new Request($"{BaseUrl}lists/{ListID}/members");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                string maxResultsString = parameters[nameof(MaxResults)];
                _ = int.TryParse(maxResultsString, out var maxResults);
                MaxResults = maxResults;
                urlParams.Add(new QueryParameter("max_results", maxResultsString));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// RetweetedBy URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildRetweetedByUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(ID), val => ID = val);

            var req = new Request($"{BaseUrl}tweets/{ID}/retweeted_by");

            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MediaFields)))
            {
                MediaFields = parameters[nameof(MediaFields)];
                urlParams.Add(new QueryParameter("media.fields", MediaFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(PlaceFields)))
            {
                PlaceFields = parameters[nameof(PlaceFields)];
                urlParams.Add(new QueryParameter("place.fields", PlaceFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(PollFields)))
            {
                PollFields = parameters[nameof(PollFields)];
                urlParams.Add(new QueryParameter("poll.fields", PollFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// builds parameters common to timeline queries
        /// </summary>
        /// <param name="parameters">parameters to process</param>
        /// <param name="req"><see cref="Request"/> object</param>
        void BuildFollowParameters(Dictionary<string, string> parameters, Request req)
        {
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                MaxResults = int.Parse(parameters[nameof(MaxResults)]);
                urlParams.Add(new QueryParameter("max_results", MaxResults.ToString()));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
            }

            BuildSharedUrlParameters(urlParams, parameters);
        }

        /// <summary>
        /// builds a url to search for user info by username(s)
        /// </summary>
        /// <param name="parameters">url parameters</param>
        /// <returns>new url for request</returns>
        Request BuildUsernameLookupUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "users/by");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Usernames)))
            {
                Usernames = parameters[nameof(Usernames)];
                urlParams.Add(new QueryParameter("usernames", Usernames.Replace(" ", "")));
            }
            else
            {
                throw new ArgumentNullException(nameof(Usernames), $"{nameof(Usernames)} is required!");
            }

            BuildSharedUrlParameters(urlParams, parameters);

            return req;
        }

        /// <summary>
        /// Appends parameters for User requests
        /// </summary>
        /// <param name="urlParams">List of parameters to build</param>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        void BuildSharedUrlParameters(IList<QueryParameter> urlParams, Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }
        }

        Request BuildSpaceBuyersUrl(Dictionary<string, string> parameters)
        {
            RequestProcessorHelper.SetSegment(parameters, nameof(SpaceID), val => SpaceID = val);

            var req = new Request($"{BaseUrl}spaces/{SpaceID}/buyers");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MediaFields)))
            {
                MediaFields = parameters[nameof(MediaFields)];
                urlParams.Add(new QueryParameter("media.fields", MediaFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(PlaceFields)))
            {
                PlaceFields = parameters[nameof(PlaceFields)];
                urlParams.Add(new QueryParameter("place.fields", PlaceFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(PollFields)))
            {
                PollFields = parameters[nameof(PollFields)];
                urlParams.Add(new QueryParameter("poll.fields", PollFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of <see cref="TwitterUserQuery"/>
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Search</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<TwitterUserQuery> user;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                user = new List<TwitterUserQuery> { new TwitterUserQuery() };
            }
            else
            {
                var userResult = JsonDeserialize(responseJson);
                user = new List<TwitterUserQuery> { userResult };
            }

            return user.OfType<T>().ToList();
        }

        TwitterUserQuery JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new TweetMediaTypeConverter(),
                    new TweetReplySettingsConverter()
                }
            };
            TwitterUserQuery? user = JsonSerializer.Deserialize<TwitterUserQuery>(responseJson, options);

            if (user == null)
                return new TwitterUserQuery
                {
                    Type = Type,
                    ID = ID,
                    Ids = Ids,
                    Expansions = Expansions,
                    ListID = ListID,
                    MaxResults = MaxResults,
                    PaginationToken = PaginationToken,
                    SpaceID = SpaceID,
                    TweetFields = TweetFields,
                    UserFields = UserFields,
                    Usernames = Usernames
                };
            else
                return user with
                {
                    Type = Type,
                    ID = ID,
                    Ids = Ids,
                    Expansions = Expansions,
                    ListID = ListID,
                    MaxResults = MaxResults,
                    PaginationToken = PaginationToken,
                    SpaceID= SpaceID,
                    TweetFields = TweetFields,
                    UserFields = UserFields,
                    Usernames = Usernames
                };
        }
    }
}