using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// A single category for Twitter suggested categories
    /// </summary>
    public class Category
    {
        public Category() { }
        public Category(JsonData catJson)
        {
            Size = catJson.GetValue<int>("size");
            Name = catJson.GetValue<string>("name");
            Slug = catJson.GetValue<string>("slug");

            var users = catJson.GetValue<JsonData>("users");
            if (users != null)
                Users =
                    (from JsonData user in users
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
