using LinqToTwitter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public record Counts
    {
        //
        // Input parameters
        //

        /// <summary>
        /// type of count <see cref="CountType"/>
        /// </summary>
        public CountType Type { get; init; }

        /// <summary>
        /// Date/Time to search to
        /// </summary>
        public DateTime EndTime { get; init; }

        /// <summary>
        /// Comma-separated list of expansion fields
        /// </summary>
        public Granularity Granularity { get; init; }

        /// <summary>
        /// Provide this, when paging, to get the next page of results
        /// </summary>
        public string? NextToken { get; init; }

        /// <summary>
        /// search query
        /// </summary>
        public string? Query { get; init; }

        /// <summary>
        /// Return tweets whose IDs are greater than this
        /// </summary>
        public string? SinceID { get; init; }

        /// <summary>
        /// Date/Time to start search
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// Return tweets whose ids are less than this
        /// </summary>
        public string? UntilID { get; init; }

        //
        // Output results
        //

        /// <summary>
        /// Tweet data returned from the search
        /// </summary>
        [JsonPropertyName("data")]
        public List<CountRange>? CountRanges { get; init; }

        /// <summary>
        /// Count metadata returned from query
        /// </summary>
        [JsonPropertyName("meta")]
        public CountsMeta? Meta { get; init; }
    }
}
