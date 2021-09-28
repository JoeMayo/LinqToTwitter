using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public record Mute
    {
        //
        // Input parameters
        //

        /// <summary>
        /// Type of mute query to perform.
        /// </summary>
        public MuteType Type { get; set; }

        /// <summary>
        /// ID of user to get mutes for
        /// </summary>
        public string? ID { get; set; }

        //
        // Output results
        //

        /// <summary>
        /// List of User that are muted, populated by List query
        /// </summary>
        [JsonPropertyName("data")]
        public List<TwitterUser>? Users { get; set; }

        /// <summary>
        /// Results metadata
        /// </summary>
        [JsonPropertyName("meta")]
        public MuteMeta? Meta { get; set; }
    }
}
