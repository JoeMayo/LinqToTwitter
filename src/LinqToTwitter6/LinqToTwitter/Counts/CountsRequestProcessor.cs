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
    public class CountsRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of count
        /// </summary>
        public CountType Type { get; set; }

        /// <summary>
        /// Date/Time to search to
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Grouping of Day, Hour, or Minute - <see cref="ExpansionField"/>
        /// </summary>
        public Granularity Granularity { get; set; }

        /// <summary>
        /// Provide this, when paging, to get the next page of results
        /// </summary>
        public string? NextToken { get; set; }

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
        /// Return tweets whose ids are less than this
        /// </summary>
        public string? UntilID { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<Counts>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(EndTime),
                       nameof(Granularity),
                       nameof(NextToken),
                       nameof(Query),
                       nameof(SinceID),
                       nameof(StartTime),
                       nameof(UntilID)
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
                Type = RequestProcessorHelper.ParseEnum<CountType>(parameters[nameof(Type)]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            string urlSegment = Type switch
            {
                CountType.All => "tweets/counts/all",
                CountType.Recent => "tweets/counts/recent",
                _ => throw new ArgumentException($"Unknown CountType: '{Type}'")
            };

            return BuildCountUrlParameters(parameters, urlSegment);
        }

        /// <summary>
        /// appends parameters for Count request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildCountUrlParameters(Dictionary<string, string> parameters, string url)
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

            if (parameters.ContainsKey(nameof(Granularity)))
            {
                Granularity = RequestProcessorHelper.ParseEnum<Granularity>(parameters[nameof(Granularity)]);
                urlParams.Add(new QueryParameter("granularity", Granularity.ToString().ToLower()));
            }

            if (parameters.ContainsKey(nameof(NextToken)))
            {
                NextToken = parameters[nameof(NextToken)];
                urlParams.Add(new QueryParameter("next_token", NextToken));
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

            return req;
        }

        /// <summary>
        /// Transforms response from Twitter into List of Search
        /// </summary>
        /// <param name="responseJson">Json response from Twitter</param>
        /// <returns>List of Search</returns>
        public virtual List<T> ProcessResults(string responseJson)
        {
            IEnumerable<Counts> counts;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                counts = new List<Counts> { new Counts() };
            }
            else
            {
                var countsResult = JsonDeserialize(responseJson);
                counts = new List<Counts> { countsResult };
            }

            return counts.OfType<T>().ToList();
        }

        Counts JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new TweetMediaTypeConverter()
                }
            };
            Counts? counts = JsonSerializer.Deserialize<Counts>(responseJson, options);

            if (counts == null)
                return new Counts()
                {
                    Type = Type,
                    EndTime = EndTime,
                    Granularity = Granularity,
                    NextToken = NextToken,
                    Query = Query,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    UntilID = UntilID,
                };
            else
                return counts with
                {
                    Type = Type,
                    EndTime = EndTime,
                    Granularity = Granularity,
                    NextToken = NextToken,
                    Query = Query,
                    SinceID = SinceID,
                    StartTime = StartTime,
                    UntilID = UntilID,
                };
        }
    }
}