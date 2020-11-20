using LinqToTwitter.Provider;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// Handles streaming queries
    /// </summary>
    /// <typeparam name="T">Streaming type</typeparam>
    public class StreamingRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWithAction<Streaming>
    {
        /// <summary>
        /// Twitter base url
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Manages communication logic for streaming
        /// </summary>
        public ITwitterExecute? TwitterExecutor { get; set; }

        /// <summary>
        /// Stream method
        /// </summary>
        public StreamingType Type { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the media object - <see cref="MediaField"/>
        /// </summary>
        public string? MediaFields { get; set; }

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
        /// Comma-separated list of rule ids, for filter rules queries
        /// </summary>
        public string? Ids { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var parameters =
               new ParameterFinder<Streaming>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(Expansions),
                       nameof(MediaFields),
                       nameof(PlaceFields),
                       nameof(PollFields),
                       nameof(TweetFields),
                       nameof(UserFields),
                       nameof(Ids)
                   }).Parameters;

            return parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to Twitter API</returns>
        public Request BuildUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(Type)))
                Type = RequestProcessorHelper.ParseEnum<StreamingType>(parameters["Type"]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            string segment = Type == StreamingType.Filter ? "tweets/search/stream" : "tweets/sample/stream";

            bool isStreaming = true;

            if (Type == StreamingType.Rules)
            {
                segment = "tweets/search/stream/rules";
                isStreaming = false;
            }

            var req = new Request(BaseUrl + segment) { IsStreaming = isStreaming };
            var urlParams = req.RequestParameters;

            BuildUrlParameters(parameters, urlParams);

            return req;
        }

        /// <summary>
        /// appends parameters for Tweet request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        void BuildUrlParameters(Dictionary<string, string> parameters, IList<QueryParameter> urlParams)
        {
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

            if (parameters.ContainsKey(nameof(Ids)))
            {
                Ids = parameters[nameof(Ids)];
                urlParams.Add(new QueryParameter("ids", Ids.Replace(" ", "")));
            }
        }

        /// <summary>
        /// Returns an object for interacting with stream
        /// </summary>
        /// <param name="notUsed">Not used</param>
        /// <returns>List with a single Streaming</returns>
        public List<T> ProcessResults(string responseJson)
        {
            IEnumerable<Streaming> streamingList;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                streamingList = new List<Streaming> { new Streaming() };
            }
            else
            {
                var tweetResult = JsonDeserialize(responseJson);
                streamingList = new List<Streaming> { tweetResult };
            }

            return streamingList.OfType<T>().ToList();
        }

        Streaming JsonDeserialize(string responseJson)
        {
            Streaming? streaming = JsonSerializer.Deserialize<Streaming>(responseJson);

            if (streaming == null)
                return new Streaming
                {
                    Type = Type,
                    Expansions = Expansions,
                    MediaFields = MediaFields,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
            else
                return streaming with
                {
                    Type = Type,
                    Expansions = Expansions,
                    MediaFields = MediaFields,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
        }

        [return: MaybeNull]
        public Streaming ProcessActionResult(string twitterResponse, Enum theAction)
        {
            return JsonDeserialize(twitterResponse);
        }
    }
}
