using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LinqToTwitter.Provider
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Takes a single element and returns an IEnumerable of just that element.
        /// </summary>
        /// <typeparam name="T">What kind of element we're enumerating</typeparam>
        /// <param name="oneOff">The one element in the resulting enumeration</param>
        /// <returns>An IEnumerable that has one element (consisting of the oneOff parameter)</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T oneOff)
        {
            yield return oneOff;
        }

        [return: MaybeNull]
        public static TTo ItemCast<TFrom, TTo>(this TFrom item, TTo? defaultValue)
            where TTo: class
        {
            return item as TTo ?? defaultValue;
        }
    }
}
