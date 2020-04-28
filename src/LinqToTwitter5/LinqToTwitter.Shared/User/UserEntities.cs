using LinqToTwitter.Common;
using LitJson;
using Newtonsoft.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// URLs associated with the User profile
    /// </summary>
    public class UserEntities
    {
        public UserEntities(JsonData entities)
        {
            Url = new Entities(entities.GetValue<JsonData>("url"));
            Description = new Entities(entities.GetValue<JsonData>("description"));
        }

        /// <summary>
        /// Url entities in the profile
        /// </summary>
        [JsonProperty("url")]
        public Entities Url { get; set; }

        /// <summary>
        /// Url entities in the description
        /// </summary>
        [JsonProperty("description")]
        public Entities Description { get; set; }
    }
}
