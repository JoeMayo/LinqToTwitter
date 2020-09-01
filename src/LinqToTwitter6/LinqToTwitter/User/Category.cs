using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// A single category for Twitter suggested categories
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Category
    {
        public Category() { }
        public Category(JsonElement catJson)
        {
            Size = catJson.GetProperty("size").GetInt32();
            Name = catJson.GetProperty("name").GetString();
            Slug = catJson.GetProperty("slug").GetString();

            var users = catJson.GetProperty("users");
            if (!users.IsNull())
                Users =
                    (from user in users.EnumerateArray()
                     select new User(user))
                    .ToList();
        }

        /// <summary>
        /// Category name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Number of users in category
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Users in category
        /// </summary>
        public List<User> Users { get; set; }
    }
}
