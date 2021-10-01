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
    /// Processes <see cref="ComplianceQuery"/> queries
    /// </summary>
    public class ComplianceRequestProcessor<T> : IRequestProcessor<T>, IRequestProcessorWantsJson
    {
        /// <summary>
        /// base url for request
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// type of tweet
        /// </summary>
        public ComplianceType Type { get; set; }

        /// <summary>
        /// UTC date/time to search to
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// ID for a single job query
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        /// Date to search from
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Comma-separated list of job statuses - <see cref="ComplianceStatus"/>
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            var paramFinder =
               new ParameterFinder<ComplianceQuery>(
                   lambdaExpression.Body,
                   new List<string> {
                       nameof(Type),
                       nameof(EndTime),
                       nameof(ID),
                       nameof(StartTime),
                       nameof(Status)
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
                Type = RequestProcessorHelper.ParseEnum<ComplianceType>(parameters[nameof(Type)]);
            else
                throw new ArgumentException($"{nameof(Type)} is required", nameof(Type));

            switch (Type)
            {
                case ComplianceType.MultipleJobs:
                    return BuildMultipleJobsUrlParameters(parameters);
                case ComplianceType.SingleJob:
                    return BuildSingleJobUrl(parameters);
                default:
                    throw new InvalidOperationException("The default case of BuildUrl should never execute because a Type must be specified.");
            }
        }

        /// <summary>
        /// appends parameters for multiple jobs request
        /// </summary>
        /// <param name="parameters">list of parameters from expression tree</param>
        /// <param name="url">base url</param>
        /// <returns>base url + parameters</returns>
        Request BuildMultipleJobsUrlParameters(Dictionary<string, string> parameters)
        {
            var req = new Request(BaseUrl + "tweets/compliance/jobs");
            var urlParams = req.RequestParameters;


            if (parameters.ContainsKey(nameof(EndTime)))
            {
                EndTime = DateTime.Parse(parameters[nameof(EndTime)]);
                urlParams.Add(new QueryParameter("end_time", EndTime.ToString(L2TKeys.ISO8601, CultureInfo.InvariantCulture)));
            }


            if (parameters.ContainsKey(nameof(StartTime)))
            {
                StartTime = DateTime.Parse(parameters[nameof(StartTime)]);
                urlParams.Add(new QueryParameter("start_time", StartTime.ToString(L2TKeys.ISO8601, CultureInfo.InvariantCulture)));
            }

            if (parameters.ContainsKey(nameof(Status)))
            {
                Status = parameters[nameof(Status)];
                urlParams.Add(new QueryParameter("status", Status.Replace(" ", "")));
            }

            return req;
        }

        /// <summary>
        /// Mentions timeline URL
        /// </summary>
        /// <param name="parameters">Parameters to process</param>
        /// <returns><see cref="Request"/> object</returns>
        Request BuildSingleJobUrl(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey(nameof(ID)))
                ID = parameters[nameof(ID)];
            else
                throw new ArgumentException($"{nameof(ID)} is required", nameof(ID));

            var req = new Request($"{BaseUrl}tweets/compliance/jobs/{ID}");

            return req;
        }


        public List<T> ProcessResults(string responseJson)
        {
            IEnumerable<ComplianceQuery> complianceQuery;

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                complianceQuery = new List<ComplianceQuery> { new ComplianceQuery() };
            }
            else
            {
                var result = JsonDeserialize(responseJson);
                complianceQuery = new List<ComplianceQuery> { result };
            }

            return complianceQuery.OfType<T>().ToList();
        }

        ComplianceQuery JsonDeserialize(string responseJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            ComplianceQuery? complianceQuery = null;

            if (Type == ComplianceType.MultipleJobs)
            {
                complianceQuery = JsonSerializer.Deserialize<ComplianceQuery>(responseJson, options);
            }
            else
            {
                complianceQuery = new ComplianceQuery();

                ComplianceJob? job = JsonSerializer.Deserialize<ComplianceJob>(responseJson, options);
                if (job is not null)
                {
                    complianceQuery = complianceQuery with
                    {
                        Jobs = new List<ComplianceJob> { job }
                    };
                };
            }

            if (complianceQuery == null)
                return new ComplianceQuery
                {
                    Type = Type,
                    EndTime = EndTime,
                    ID = ID,
                    StartTime = StartTime,
                    Status = Status
                };
            else
                return complianceQuery with
                {
                    Type = Type,
                    EndTime = EndTime,
                    ID = ID,
                    StartTime = StartTime,
                    Status = Status
                };
        }
    }
}