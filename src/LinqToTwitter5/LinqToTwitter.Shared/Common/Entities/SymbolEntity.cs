using Newtonsoft.Json;
using System;

namespace LinqToTwitter
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
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Locations for begin/end index of where symbol occurs.
        /// </summary>
        [JsonProperty("indices")]
        public int[] Indices { get; set; }
    }
}
