using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record CountRange
    {
        [JsonPropertyName("end")]
        public DateTime End { get; set; }

        [JsonPropertyName("start")]
        public DateTime Start { get; set; }

        [JsonPropertyName("tweet_count")]
        public int TweetCount { get; set; }
    }
}