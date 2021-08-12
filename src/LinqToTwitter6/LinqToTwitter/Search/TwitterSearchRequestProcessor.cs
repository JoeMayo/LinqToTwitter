using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToTwitter.Common;
using LinqToTwitter.Provider;

namespace LinqToTwitter
{
    /// <summary>
    /// processes search queries
    /// </summary>
    public class TwitterSearchRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of search
        /// </summary>
        public SearchType Type { get; set; }

        /// <summary>
        /// Date/Time to search to
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Maximum number of tweets to return
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the media object - <see cref="MediaField"/>
        /// </summary>
        public string? MediaFields { get; set; }

        /// <summary>
        /// Provide this, when paging, to get the next page of results
        /// </summary>
        public string? NextToken { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the place object - <see cref="PlaceField"/>
        /// </summary>
        public string? PlaceFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the poll object - <see cref="PollField"/>
        /// </summary>
        public string? PollFields { get; set; }

        /// <summary>
        /// search query
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Return tweets whose IDs are greater than this
        /// </summary>
        public string? SinceID { get; set; }

        /// <summary>
        /// Date/Time to start search
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object - <see cref="TweetField"/>
        /// </summary>
        public string? TweetFields { get; set; }

        /// <summary>
        /// Return tweets whose ids are less than this
        /// </summary>
        public string? UntilID { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<TwitterSearch>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(EndTime),
                       nameof(Expansions),
                       nameof(MaxResults),
                       nameof(MediaFields),
                       nameof(NextToken),
                       nameof(PlaceFields),
                       nameof(PollFields),
                       nameof(Query),
                       nameof(SinceID),
                       nameof(StartTime),
                       nameof(TweetFields),
                       nameof(UntilID),
                       nameof(UserFields)
                   }) ;

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
                Type = RequestProcessorHelper.ParseEnum<SearchType>(parameters[nameof(Type)]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            string urlSegment = Type switch
            {
                SearchType.FullSearch => "tweets/search/all",
                SearchType.RecentSearch => "tweets/search/recent",
                _ => throw new ArgumentException($"Unknown SearchType: '{Type}'")
            };

            return BuildSearchUrlParameters(parameters, urlSegment);
        }

        /// <summary>
        /// appends parameters for Search request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildSearchUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
            var urlParams = req.RequestParameters;


            if (parameters.ContainsKey(nameof(Query)) && !string.IsNullOrWhiteSpace(parameters[nameof(Query)]))
            {
                Query = parameters[nameof(Query)];
                urlParams.Add(new QueryParameter("query", Query));
            }
            else
            {
                throw new ArgumentNullException(nameof(Query), "Query filter in where clause is required.");
            }

            if (parameters.ContainsKey(nameof(EndTime)))
            {
                EndTime = DateTime.Parse(parameters[nameof(EndTime)]);
                urlParams.Add(new QueryParameter("end_time", EndTime.ToString(L2TKeys.ISO8601, CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(Expansions)))
            {
                Expansions = parameters[nameof(Expansions)];
                urlParams.Add(new QueryParameter("expansions", Expansions.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(MaxResults)))
            {
                MaxResults = int.Parse(parameters[nameof(MaxResults)]);
                urlParams.Add(new QueryParameter("max_results", MaxResults.ToString(CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(MediaFields)))
            {
                MediaFields = parameters[nameof(MediaFields)];
                urlParams.Add(new QueryParameter("media.fields", MediaFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(NextToken)))
            {
                NextToken = parameters[nameof(NextToken)];
                urlParams.Add(new QueryParameter("next_token", NextToken));
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

            if (parameters.ContainsKey(nameof(SinceID)))
            {
                SinceID = parameters[nameof(SinceID)];
                urlParams.Add(new QueryParameter("since_id", SinceID));
            }

            if (parameters.ContainsKey(nameof(StartTime)))
            {
                StartTime = DateTime.Parse(parameters[nameof(StartTime)]);
                urlParams.Add(new QueryParameter("start_time", StartTime.ToString(L2TKeys.ISO8601, CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(TweetFields)))
            {
                TweetFields = parameters[nameof(TweetFields)];
                urlParams.Add(new QueryParameter("tweet.fields", TweetFields.Replace(" ", "")));
            }

            if (parameters.ContainsKey(nameof(UntilID)))
            {
                UntilID = parameters[nameof(UntilID)];
                urlParams.Add(new QueryParameter("until_id", UntilID));
            }

            if (parameters.ContainsKey(nameof(UserFields)))
            {
                UserFields = parameters[nameof(UserFields)];
                urlParams.Add(new QueryParameter("user.fields", UserFields.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Search
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Search</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<TwitterSearch> search;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                search = new List<TwitterSearch> { new TwitterSearch() };
            }
            else
            {
                var searchResult = JsonDeserialize(responseJson);
                search = new List<TwitterSearch> { searchResult };
            }

            return search.OfType<T>().ToList();
        }

        TwitterSearch JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new TweetMediaTypeConverter()
                }
            };
            TwitterSearch? search = JsonSerializer.Deserialize<TwitterSearch>(responseJson, options);

            if (search == null)
                return new TwitterSearch
                {
                    Type = Type,
                    EndTime = EndTime,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    NextToken = NextToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    Query = Query,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    TweetFields = TweetFields,
                    UntilID = UntilID,
                    UserFields = UserFields
                };
            else
                return search with
                {
                    Type = Type,
                    EndTime = EndTime,
                    Expansions = Expansions,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    NextToken = NextToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    Query = Query,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    TweetFields = TweetFields,
                    UntilID = UntilID,
                    UserFields = UserFields
                };
        }
    }
}