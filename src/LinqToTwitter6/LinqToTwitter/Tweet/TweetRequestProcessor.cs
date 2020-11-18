using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
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
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Required - Up to 100 comma-separated IDs to search for
        /// </summary>
        public string? Ids { get; set; }

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
                       nameof(Ids),
                       nameof(Expansions),
                       nameof(MediaFields),
                       nameof(PlaceFields),
                       nameof(PollFields),
                       nameof(TweetFields),
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

            return BuildUrlParameters(parameters, "tweets");
        }

        /// <summary>
        /// appends parameters for <see cref="Tweet"/> request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildUrlParameters(Dictionary<string, string> parameters, string url)
        {
            var req = new Request(BaseUrl + url);
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
            TweetQuery? tweet = JsonSerializer.Deserialize<TweetQuery>(responseJson);

            if (tweet == null)
                return new TweetQuery
                {
                    Type = Type,
                    Ids = Ids,
                    Expansions = Expansions,
                    MediaFields = MediaFields,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
            else
                return tweet with
                {
                    Type = Type,
                    Ids = Ids,
                    Expansions = Expansions,
                    MediaFields = MediaFields,
                    PlaceFields = PlaceFields,
                    PollFields = PollFields,
                    TweetFields = TweetFields,
                    UserFields = UserFields
                };
        }
    }
}