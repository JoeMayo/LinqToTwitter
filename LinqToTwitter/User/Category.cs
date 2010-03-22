using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// A single category for Twitter suggested categories
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Converts XML to Category
        /// </summary>
        /// <param name="category">XML with category info</param>
        /// <returns>Category with info from XML</returns>
        public Category CreateCategory(XElement category)
        {
            if (category == null)
            {
                return null;
            }

            return new Category
            {
                 Name =
                    category.Element("name") == null ?
                        string.Empty :
                        category.Element("name").Value,
                 Categories =
                    (from cat in category.Elements("category")
                     select CreateCategory(cat))
                     .ToList(),
                 Slug =
                    category.Element("slug") == null ?
                        string.Empty :
                        category.Element("slug").Value,
                 Users =
                    category.Element("users") == null ?
                    null :
                    (from user in category.Element("users").Elements("user")
                     select new User().CreateUser(user))
                     .ToList()
            };
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
        /// ?
        /// </summary>
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Users in category
        /// </summary>
        public List<User> Users { get; set; }
    }
}
