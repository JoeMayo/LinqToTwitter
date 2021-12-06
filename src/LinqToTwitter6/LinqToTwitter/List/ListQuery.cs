using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Request and Response for List queries
    /// </summary>
    public record ListQuery
    {
        //
        // These are the available input parameters, depending on query type
        //

        /// <summary>
        /// type of list to query
        /// </summary>
        public ListType Type { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Comma-separated list of list fields - <see cref="ListFields"/>
        /// </summary>
        public string? ListFields { get; set; }

        /// <summary>
        /// List ID
        /// </summary>
        public string? ListID { get; set; }

        /// <summary>
        /// Max number of results
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Used to get the next page of results
        /// </summary>
        public string? PaginationToken { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string? UserID { get; set; }

        //
        // Output returned from query
        //

        /// <summary>
        /// Lists response
        /// </summary>
        [JsonConverter(typeof(ListConverter))]
        [JsonPropertyName("data")]
        public List<List>? Lists { get; init; }

        /// <summary>
        /// Include data, depends on query
        /// </summary>
        [JsonPropertyName("includes")]
        public TwitterInclude? Includes { get; set; }

        /// <summary>
        /// Metadata with count and paging details
        /// </summary>
        [JsonPropertyName("meta")]
        public ListMeta? Meta { get; set; }
    }
}
