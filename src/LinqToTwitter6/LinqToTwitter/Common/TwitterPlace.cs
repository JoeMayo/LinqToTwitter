using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    public record TwitterPlace
    {
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("place_type")]
        public string? PlaceType { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public string? ID { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("geo")]
        public Geo? Geo { get; set; }
    }
}
