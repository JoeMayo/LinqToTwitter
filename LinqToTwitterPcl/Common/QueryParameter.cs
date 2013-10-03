using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    /// <summary>
    /// Provides an structure to hold the query parameters
    /// </summary>
    public class QueryParameter : IComparable<QueryParameter>, IComparable
    {
        private readonly string name = null;
        private string value = null;

        public QueryParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
            internal set { this.value = value; }
        }

        public int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return Object.ReferenceEquals(this, null) ? 0 : 1;

            var other = obj as QueryParameter;
            return CompareTo(other);
        }

        public int CompareTo(QueryParameter other)
        {
            return defaultComparer.Compare(this, other);
        }

        public static IComparer<QueryParameter> defaultComparer = new QueryParameterComparer();
    }

    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
    {
        public int Compare(QueryParameter x, QueryParameter y)
        {
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