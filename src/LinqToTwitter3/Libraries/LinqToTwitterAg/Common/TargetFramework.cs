using System;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// Utility methods for normalizing differences between 
    /// different .NET Framework Target versions
    /// </summary>
    public class TargetFramework
    {
        /// <summary>
        /// Safely parses a string into an Enum
        /// </summary>
        /// <remarks>
        /// .NET 3.5 doesn't have Enum.TryParse. This method catches exceptions and returns a default value.  
        /// If you're interested in potential exceptions, you should use System.Enum.Parse.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed enum</returns>
        public static T ParseEnum<T>(string value)
            where T : struct
        {
            return ParseEnum(value, true, default(T));
        }

        /// <summary>
        /// Safely parses a string into an Enum
        /// </summary>
        /// <remarks>
        /// .NET 3.5 doesn't have Enum.TryParse. This method catches exceptions and returns a default value.  
        /// If you're interested in potential exceptions, you should use System.Enum.Parse.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to parse</param>
        /// <param name="ignoreCase">Do case-insensitive parse</param>
        /// <param name="defaultValue">Default</param>
        /// <returns>Parsed enum</returns>
        public static T ParseEnum<T>(string value, bool ignoreCase, T defaultValue)
            where T : struct
        {
            T parsedVal;

            try
            {
                parsedVal = 
                    !string.IsNullOrEmpty(value.Trim()) ?
                        (T)Enum.Parse(typeof (T), value, ignoreCase) :
                        defaultValue;
            }
            catch
            {
                parsedVal = defaultValue;
            }

            return parsedVal;
        }
    }
}
