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
                       nameof(StartTime),
                       nameof(Status)
                   }) ;

            return paramFinder.Parameters;
        }

        public Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            throw new NotImplementedException();
        }

        public List<T> ProcessResults(string twitterResponse)
        {
            throw new NotImplementedException();
        }
    }
}