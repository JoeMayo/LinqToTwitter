using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public class ComplianceQuery
    {
        //
        // Query input fields
        //

        /// <summary>
        /// type of compliance job query
        /// </summary>
        public ComplianceType Type { get; init; }

        /// <summary>
        /// UTC date/time to search to
        /// </summary>
        public DateTime EndTime { get; init; }

        /// <summary>
        /// Date to search from
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// Comma-separated list of job statuses
        /// </summary>
        public string? Status { get; init; }

        //
        // Output results
        //

        /// <summary>
        /// Compliance job data returned from the search
        /// </summary>
        [JsonPropertyName("jobs")]
        public List<ComplianceJob>? Jobs { get; init; }
    }
}
