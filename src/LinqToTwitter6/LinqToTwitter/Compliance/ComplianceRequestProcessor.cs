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
        /// type of compliance job
        /// </summary>
        public ComplianceType Type { get; set; }

        /// <summary>
        /// ID for a single job query
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        /// Type of compliance job to query (tweets or users)
        /// </summary>
        public string? JobType { get; set; }

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
                       nameof(ID),
                       nameof(JobType),
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
            var req = new Request(BaseUrl + "compliance/jobs");
            var urlParams = req.RequestParameters;

            if (parameters.ContainsKey(nameof(JobType)))
            {
                JobType = parameters[nameof(JobType)];
                urlParams.Add(new QueryParameter("type", JobType));
            }
            else
            {
                throw new ArgumentException($"{nameof(JobType)} is required", nameof(JobType));
            }

            if (parameters.ContainsKey(nameof(Status)))
            {
                Status = parameters[nameof(Status)];
                urlParams.Add(new QueryParameter("status", Status));
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

            var req = new Request($"{BaseUrl}compliance/jobs/{ID}");

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
                ComplianceQuerySingle? singleQuery = JsonSerializer.Deserialize<ComplianceQuerySingle>(responseJson, options);
                if (singleQuery?.Job is not null)
                {
                    complianceQuery = new ComplianceQuery
                    {
                        Jobs = new List<ComplianceJob> { singleQuery.Job }
                    };
                };
            }

            if (complianceQuery == null)
                return new ComplianceQuery
                {
                    Type = Type,
                    ID = ID,
                    JobType = JobType,
                    Status = Status
                };
            else
                return complianceQuery with
                {
                    Type = Type,
                    ID = ID,
                    JobType = JobType,
                    Status = Status
                };
        }
    }
}