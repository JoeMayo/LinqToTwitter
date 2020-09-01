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
            Url = new Entities(entities.GetProperty("url"));
            Description = new Entities(entities.GetProperty("description"));
        }

        /// <summary>
        /// Url entities in the profile
        /// </summary>
        public Entities Url { get; set; }

        /// <summary>
        /// Url entities in the description
        /// </summary>
        public Entities Description { get; set; }
    }
}
