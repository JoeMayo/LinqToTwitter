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
    /// Processes <see cref="Tweet"/> queries
    /// </summary>
    public class TweetRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of tweet
        /// </summary>
        public TweetType Type { get; set; }

        /// <summary>
        /// UTC date/time to search to
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Comma-separated list of tweet types to exclude
        /// </summary>
        public string? Exclude { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Required - Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; set; }

        /// <summary>
        /// User ID for timeline queries
        /// </summary>
        public string? ID { get; set; }

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
        /// returns tweets later than this ID
        /// </summary>
        public string? SinceID { get; set; }

        /// <summary>
        /// Date to search from
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Tweet object - <see cref="TweetField"/>
        /// </summary>
        public string? TweetFields { get; set; }

        /// <summary>
        /// returns tweets earlier than this ID
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
               new ParameterFinder<TweetQuery>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(EndTime),
                       nameof(Exclude),
                       nameof(Expansions),
                       nameof(Ids),
                       nameof(ID),
                       nameof(MaxResults),
                       nameof(MediaFields),
                       nameof(PaginationToken),
                       nameof(PlaceFields),
                       nameof(PollFields),
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
                Type = RequestProcessorHelper.ParseEnum<TweetType>(parameters[nameof(Type)]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            Type = RequestProcessorHelper.ParseEnum<TweetType>(parameters[nameof(Type)]);

            switch (Type)
            {
                case TweetType.Lookup:
                    return BuildLookupUrl(parameters);
                case TweetType.MentionsTimeline:
                    return BuildMentionsTimelineUrl(parameters);
                case TweetType.TweetsTimeline:
                    return BuildUserTimelineUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }

        }

        /// <summary>
        /// Lookup URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildLookupUrl(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "tweets");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(Ids)))
            {
                Ids = parameters[nameof(Ids)];
                urlParams.Add(new QueryParameter("ids", Ids.Replace(" ", "")));
            }
            else
            {
                throw new ArgumentException($"{nameof(Ids)} is required", nameof(Ids));
            }

            BuildUrlFieldParameters(parameters, req);

            return req;
        }

        /// <summary>
        /// Mentions timeline URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildMentionsTimelineUrl(Dictionary<string, string> parameters)
        {
            SetUserID(parameters);

            var req = new Request($"{BaseUrl}users/{ID}/mentions");

            BuildTimelineParameters(parameters, req);

            return req;
        }

        /// <summary>
        /// User timeline URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildUserTimelineUrl(Dictionary<string, string> parameters)
        {
            SetUserID(parameters);

            var req = new Request($"{BaseUrl}users/{ID}/tweets");

            BuildTimelineParameters(parameters, req);

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
        /// builds parameters common to timeline queries
        /// </summary>
        /// <param name="parameters">parameters to process</param>
        /// <param name="req"><see cref="Request"/> object</param>
        void BuildTimelineParameters(Dictionary<string, string> parameters, Request req)
        {
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(EndTime)))
            {
                EndTime = DateTime.Parse(parameters[nameof(EndTime)]);
                urlParams.Add(new QueryParameter("end_time", EndTime.ToString(L2TKeys.ISO8601, CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(Exclude)))
            {
                Exclude = parameters[nameof(Exclude)];
                urlParams.Add(new QueryParameter("exclude", Exclude.Replace(" ", "")));
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

            if (parameters.ContainsKey(nameof(UntilID)))
            {
                UntilID = parameters[nameof(UntilID)];
                urlParams.Add(new QueryParameter("until_id", UntilID));
            }

            BuildUrlFieldParameters(parameters, req);
        }

        /// <summary>
        /// appends parameters for expansions and fields
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="req"><see cref="Request"/>  object</param>
        void BuildUrlFieldParameters(Dictionary<string, string> parameters, Request req)
        {
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
        }

        /// <summary>
        /// Transforms response from Twitter into List of Tweets
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Tweets</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<TweetQuery> tweet;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                tweet = new List<TweetQuery> { new TweetQuery() };
            }
            else
            {
                var tweetResult = JsonDeserialize(responseJson);
                tweet = new List<TweetQuery> { tweetResult };
            }

            return tweet.OfType<T>().ToList();
        }

        TweetQuery JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new TweetMediaTypeConverter()
                }
            };
            TweetQuery? tweet = JsonSerializer.Deserialize<TweetQuery>(responseJson, options);

            if (tweet == null)
                return new TweetQuery
                {
                    Type = Type,
                    EndTime = EndTime,
                    Exclude = Exclude,
                    Expansions = Expansions,
                    ID = ID,
                    Ids = Ids,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    PaginationToken = PaginationToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    TweetFields = TweetFields,
                    UntilID = UntilID,
                    UserFields = UserFields
                };
            else
                return tweet with
                {
                    Type = Type,
                    EndTime = EndTime,
                    Exclude = Exclude,
                    Expansions = Expansions,
                    ID = ID,
                    Ids = Ids,
                    MaxResults = MaxResults,
                    MediaFields = MediaFields,
                    PaginationToken = PaginationToken,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    TweetFields = TweetFields,
                    UntilID = UntilID,
                    UserFields = UserFields
                };
        }
    }
}