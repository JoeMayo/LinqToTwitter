using System;
using System.Collections.Generic;

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// Provides an structure to hold the query parameters
    /// </summary>
    public class QueryParameter : IComparable<QueryParameter>, IComparable
    {
        public QueryParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }

        public string Value { get; internal set; }

        public int CompareTo(object? obj)
        {
            if (obj is null)
                return this is null ? 0 : 1;

            var other = obj as QueryParameter;
            return CompareTo(other);
        }

        public int CompareTo(QueryParameter? other)
        {
            return defaultComparer.Compare(this, other);
        }

        static readonly IComparer<QueryParameter> defaultComparer = new QueryParameterComparer();
    }

    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
    {
        public int Compare(QueryParameter? x, QueryParameter? y)
        {
            if (x == null || y == null)
            {
                if (x == y)
                    return 0;
                else if (x == null && y != null)
                    return 1;
                else
                    return -1;
            }

            if (x.Name.Equals(y.Name))
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }
    }
}