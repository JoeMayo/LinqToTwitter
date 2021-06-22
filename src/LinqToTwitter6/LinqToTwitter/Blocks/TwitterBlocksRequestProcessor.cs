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
    public class TwitterBlocksRequestProcessor<T> :
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
        public BlockingType? Type { get; set; }

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
        /// If set, with token from previous response metadata, pages forward or backward
        /// </summary>
        public string? PaginationToken { get; set; }

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
               new ParameterFinder<TwitterBlocksQuery>(
                   lambdaExpression.Body,
                   new List<string> { 
                       nameof(Type),
                       nameof(Expansions),
                       nameof(ID),
                       nameof(MaxResults),
                       nameof(PaginationToken),
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

            Type = RequestProcessorHelper.ParseEnum<BlockingType>(parameters["Type"]);

            switch (Type)
            {
                case BlockingType.Lookup:
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

            var req = new Request($"{BaseUrl}users/{ID}/blocking");
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
            IEnumerable<TwitterBlocksQuery> blocks;


            if (string.IsNullOrWhiteSpace(responseJson))
            {
                blocks = new List<TwitterBlocksQuery>();
            }
            else
            {
                switch (Type)
                {
                    case BlockingType.Lookup:
                        TwitterBlocksQuery blocksResult = JsonDeserialize(responseJson);
                        blocks = new List<TwitterBlocksQuery> { blocksResult };
                        break;
                    default:
                        throw new ArgumentException("Unhandled BlockingType.");
                }
            }

            return blocks.OfType<T>().ToList();
        }

        TwitterBlocksQuery JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            TwitterBlocksQuery? blocks = JsonSerializer.Deserialize<TwitterBlocksQuery>(responseJson, options);

            if (blocks?.Meta == null || blocks.Meta.ResultCount == 0)
                return new TwitterBlocksQuery
                {
                    Type = Type,
                    ID = ID,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    PaginationToken = PaginationToken,
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
                    PaginationToken = PaginationToken,
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
