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
        public string Text { get; set; }
    }
}
