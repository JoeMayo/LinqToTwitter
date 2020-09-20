using LinqToTwitter.Common;
using LinqToTwitter.Common.Entities;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// URLs associated with the User profile
    /// </summary>
    public class UserEntities
    {
        public UserEntities(JsonElement entities)
        {
            if (entities.IsNull())
                return;

            entities.TryGetProperty("url", out JsonElement urlValue);
            Url = new Entities(urlValue);
            entities.TryGetProperty("description", out JsonElement descriptionValue);
            Description = new Entities(descriptionValue);
        }

        /// <summary>
        /// Url entities in the profile
        /// </summary>
        public Entities? Url { get; set; }

        /// <summary>
        /// Url entities in the description
        /// </summary>
        public Entities? Description { get; set; }
    }
}
