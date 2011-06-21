using System;

namespace LinqToTwitterDemo
{
    static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return value.IsNullOrEmpty() || value.Trim(' ').Length == 0;
        }
    }
}
