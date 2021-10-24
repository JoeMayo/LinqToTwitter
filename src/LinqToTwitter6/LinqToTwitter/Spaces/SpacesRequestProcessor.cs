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
    /// Processes Twitter Spaces requests.
    /// </summary>
    public class SpacesRequestProcessor<T> :
        IRequestProcessor<T>,
        IRequestProcessorWantsJson,
        IRequestProcessorWithAction<T>
        where T : class
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public virtual string? BaseUrl { get; set; }

        /// <summary>
        /// Type of space query to perform.
        /// </summary>
        public SpacesType Type { get; set; }

        /// <summary>
        /// Criteria for Search queries
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Comma-separated list of creator IDs to search for
        /// </summary>
        public string? CreatorIds { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Max number of tweets to return per requrest - default 10 - possible 100
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Space object - <see cref="SpaceField"/>
        /// </summary>
        public string? SpaceFields { get; set; }

        /// <summary>
        /// Comma-separated list of space IDs to search for
        /// </summary>
        public string? SpaceIds { get; set; }

        /// <summary>
        /// Current state of the space - <see cref="SpaceState"/>
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<SpacesQuery>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Query),
                       nameof(CreatorIds),
                       nameof(Expansions),
                       nameof(MaxResults),
                       nameof(SpaceFields),
                       nameof(SpaceIds),
                       nameof(State),
                       nameof(UserFields)
                   });

            return paramFinder.Parameters;
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

            Type = RequestProcessorHelper.ParseEnum<SpacesType>(parameters["Type"]);

            return Type switch
            {
                SpacesType.ByCreatorID => BuildByCreatorIdsUrl(parameters),
                SpacesType.BySpaceID => BuildBySpaceIdsUrl(parameters),
                SpacesType.Search => BuildSearchdUrl(parameters),
                _ => throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified."),
            };
        }

        Request BuildByCreatorIdsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request($"{BaseUrl}spaces/by/creator_ids");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(CreatorIds)))
            {
                CreatorIds = parameters[nameof(CreatorIds)];
                urlParams.Add(new QueryParameter("user_ids", CreatorIds.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(SpaceFields)))
            {
                SpaceFields = parameters[nameof(SpaceFields)];
                urlParams.Add(new QueryParameter("space.fields", SpaceFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        Request BuildBySpaceIdsUrl(Dictionary<string, string> parameters)
        {
            var req = new Request($"{BaseUrl}spaces");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(SpaceIds)))
            {
                SpaceIds = parameters[nameof(SpaceIds)];
                urlParams.Add(new QueryParameter("ids", SpaceIds.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(SpaceFields)))
            {
                SpaceFields = parameters[nameof(SpaceFields)];
                urlParams.Add(new QueryParameter("space.fields", SpaceFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        Request BuildSearchdUrl(Dictionary<string, string> parameters)
        {
            var req = new Request($"{BaseUrl}spaces/search");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Query)))
            {
                Query = parameters[nameof(Query)];
                urlParams.Add(new QueryParameter("query", Query));
            }

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                MaxResults = int.Parse(parameters[nameof(MaxResults)]);
                urlParams.Add(new QueryParameter("max_results", MaxResults.ToString()));
            }

            if (parameters.ContainsKey(nameof(SpaceFields)))
            {
                SpaceFields = parameters[nameof(SpaceFields)];
                urlParams.Add(new QueryParameter("space.fields", SpaceFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(State)))
            {
                State = parameters[nameof(State)];
                urlParams.Add(new QueryParameter("state", State));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        ///// <summary>
        ///// Sets parameter, but doesn't treat as a query parameter.
        ///// </summary>
        ///// <param name="parameters">list of parameters</param>
        //void SetUserID(Dictionary<string, string> parameters)
        //{
        //    if (parameters.ContainsKey(nameof(ID)))
        //        ID = parameters[nameof(ID)];
        //    else
        //        throw new ArgumentException($"{nameof(ID)} is required", nameof(ID));
        //}

        /// <summary>
        /// Transforms Twitter response into List of User
        /// </summary>
        /// <param name="responseJson">Twitter response</param>
        /// <returns>List of User</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            if (string.IsNullOrWhiteSpace(responseJson)) return new List<T>();

            List<SpacesQuery>? spaces;

            switch (Type)
            {
                case SpacesType.Search:
                case SpacesType.ByCreatorID:
                case SpacesType.BySpaceID:
                    spaces = new List<SpacesQuery> { JsonDeserialize(responseJson) };
                    break;
                default:
                    spaces = new List<SpacesQuery>();
                    break;
            }

            return spaces.OfType<T>().ToList();
        }

        SpacesQuery JsonDeserialize(string responseJson)
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
            SpacesQuery? space = JsonSerializer.Deserialize<SpacesQuery>(responseJson, options);

            if (space == null)
                return new SpacesQuery
                {
                    Type = Type,
                    Query = Query,
                    CreatorIds = CreatorIds,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    SpaceFields = SpaceFields,
                    SpaceIds = SpaceIds,
                    State = State,
                    UserFields = UserFields
                };
            else
                return space with
                {
                    Type = Type,
                    Query = Query,
                    CreatorIds = CreatorIds,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    SpaceFields = SpaceFields,
                    SpaceIds = SpaceIds,
                    State = State,
                    UserFields = UserFields
                };
        }

        List<User> HandleSingleUserResponse(JsonElement userJson)
        {
            List<User> userList = new List<User> { new User(userJson) };
            return userList;
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement userJson = JsonDocument.Parse(responseJson).RootElement;

            List<User> user = HandleSingleUserResponse(userJson);

            return user.Single().ItemCast(default(T));
        }
    }
}
