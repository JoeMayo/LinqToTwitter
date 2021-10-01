using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes block queries
    /// </summary>
    public class LikeRequestProcessor<T> :
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
        /// type of blocks request to perform
        /// </summary>
        public LikeType? Type { get; set; }

        /// <summary>
        /// ID of user performing the block
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Max number of tweets to return per requrest - default 10 - possible 100
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
        /// Comma-separated list of fields to return in the Tweet object - <see cref="TweetField"/>
        /// </summary>
        public string? TweetFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(System.Linq.Expressions.LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<LikeQuery>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Expansions),
                       nameof(ID),
                       nameof(MaxResults),
                       nameof(MediaFields),
                       nameof(PaginationToken),
                       nameof(PlaceFields),
                       nameof(PollFields),
                       nameof(TweetFields),
                       nameof(UserFields)
                   });

            return paramFinder.Parameters;
        }

        /// <summary>
        /// Builds url based on input parameters.
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> parameters)
        {
            const string TypeParam = "Type";
            if (parameters == null || !parameters.ContainsKey("Type"))
                throw new ArgumentException("You must set Type.", TypeParam);

            Type = RequestProcessorHelper.ParseEnum<LikeType>(parameters["Type"]);

            switch (Type)
            {
                case LikeType.Lookup:
                    return BuildLookupUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// builds an url for looking up blocked users
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <returns>base url + show segment</returns>
        Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            SetUserID(parameters);

            var req = new Request($"{BaseUrl}users/{ID}/liked_tweets");
            var urlParams = req.RequestParameters;

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

            if (parameters.ContainsKey(nameof(MediaFields)))
            {
                MediaFields = parameters[nameof(MediaFields)];
                urlParams.Add(new QueryParameter("media.fields", MediaFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(PaginationToken)))
            {
                PaginationToken = parameters[nameof(PaginationToken)];
                urlParams.Add(new QueryParameter("pagination_token", PaginationToken));
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
        /// Used by timeline queries - sets parameter, but doesn't treat as a query parameter.
        /// </summary>
        /// <param name="parameters">list of parameters</param>
        void SetUserID(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(ID)))
                ID = parameters[nameof(ID)];
            else
                throw new ArgumentException($"{nameof(ID)} is required", nameof(ID));
        }

        /// <summary>
        /// Transforms twitter response into List of Blocks objects
        /// </summary>
        /// <returns>List of Blocks</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<LikeQuery> likes;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                likes = new List<LikeQuery>();
            }
            else
            {
                switch (Type)
                {
                    case LikeType.Lookup:
                        LikeQuery likesResult = JsonDeserialize(responseJson);
                        likes = new List<LikeQuery> { likesResult };
                        break;
                    default:
                        throw new ArgumentException("Unhandled LikeType.");
                }
            }

            return likes.OfType<T>().ToList();
        }

        LikeQuery JsonDeserialize(string responseJson)
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
            LikeQuery? blocks = JsonSerializer.Deserialize<LikeQuery>(responseJson, options);

            if (blocks?.Meta == null || blocks.Meta.ResultCount == 0)
                return new LikeQuery
                {
                    Type = Type,
                    ID = ID,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    PaginationToken = PaginationToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
            else
                return blocks with
                {
                    Type = Type,
                    ID = ID,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    PaginationToken = PaginationToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
        }

        public T? ProcessActionResult(string responseJson, Enum theAction)
        {
            JsonElement blocksJson = JsonDocument.Parse(responseJson).RootElement;

            var user = new User(blocksJson);

            return user.ItemCast(default(T));
        }
    }
}
