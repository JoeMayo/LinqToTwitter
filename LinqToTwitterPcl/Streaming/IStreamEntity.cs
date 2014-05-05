using System;
using System.Linq;

namespace LinqToTwitter
{
    public interface IStreamEntity
    {
        /// <summary>
        /// Raw Json from Twitter response
        /// </summary>
        string JsonContent { get; set; }

        /// <summary>
        /// Type of Stream Message
        /// </summary>
        StreamEntityType EntityType { get; set; }

        /// <summary>
        /// LINQ to Twitter entity from JsonContent
        /// </summary>
        object Entity { get; set; }
    }
}