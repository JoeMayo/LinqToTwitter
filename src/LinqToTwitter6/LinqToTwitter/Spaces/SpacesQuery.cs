using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record SpacesQuery
    {
        //
        // Input parameters
        //

        /// <summary>
        /// Type of space query to perform.
        /// </summary>
        public SpacesType Type { get; set; }

        /// <summary>
        /// Criteria for Search queries
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Comma-separated list of creator IDs to search for
        /// </summary>
        public string? CreatorIds { get; set; }

        /// <summary>
        /// Comma-separated list of expansion fields - <see cref="ExpansionField"/>
        /// </summary>
        public string? Expansions { get; set; }

        /// <summary>
        /// Max number of tweets to return per requrest - default 10 - possible 100
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Space object - <see cref="SpaceField"/>
        /// </summary>
        public string? SpaceFields { get; set; }

        /// <summary>
        /// Comma-separated list of space IDs to search for
        /// </summary>
        public string? SpaceIds { get; set; }

        /// <summary>
        /// Current state of the space - <see cref="SpaceState"/>
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the Topic object - <see cref="TopicField"/>
        /// </summary>
        public string? TopicFields { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return in the User object - <see cref="UserField"/>
        /// </summary>
        public string? UserFields { get; set; }

        //
        // Output results
        //

        /// <summary>
        /// Space data returned from the search
        /// </summary>
        [JsonPropertyName("data")]
        public List<Space>? Spaces { get; init; }

        /// <summary>
        /// Populated when query includes expansion fields
        /// </summary>
        [JsonPropertyName("includes")]
        public TwitterInclude? Includes { get; init; }

        /// <summary>
        /// Space metadata returned from search
        /// </summary>
        [JsonPropertyName("meta")]
        public SpaceMeta? Meta { get; set; }
    }
}