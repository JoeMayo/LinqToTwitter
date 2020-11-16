using System.Text.Json.Serialization;

namespace LinqToTwitter.Common.Entities
{
    /// <summary>
    /// Twitter symbol entity in the tweet
    /// </summary>
    /// <example>@linkedin</example>
    public class SymbolEntity : EntityBase
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Locations for begin/end index of where symbol occurs.
        /// </summary>
        [JsonPropertyName("indices")]
        public int[]? Indices { get; set; }
    }
}
