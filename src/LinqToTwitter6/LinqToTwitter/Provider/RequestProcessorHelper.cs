﻿using System;
using System.Collections.Generic;

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// reusable methods for all request processors
    /// </summary>
    internal class RequestProcessorHelper
    {
        /// <summary>
        /// Some query parameters represent enum types. Different languages
        /// handle such values in different ways when translating query expressions.
        /// This method performs the conversion to the enum type regardless of whether
        /// the parameter string represents an int value or a textual enum case name.
        /// </summary>
        /// <remarks>
        /// Delphi and F# enums come to the IRequestProcessor as pneumonic strings,
        /// but C# enums arrive as the underlying int type of the enum;
        /// therefore, we must determine what we're working with to succeed.
        /// </remarks>
        /// <typeparam name="T">Enum type to convert to</typeparam>
        /// <param name="enumValue">
        /// Either a string enum member name (from Delphi Prism or F#)
        /// or an underlying int value (from C#/VB)
        /// </param>
        /// <returns>Parameter value translated to the requested enum type</returns>
        internal static T ParseEnum<T>(string enumValue)
        {
            return (T)Enum.Parse(typeof(T), enumValue, /*ignoreCase:*/ true);
        }

        /// <summary>
        /// Determines if a parameter is true
        /// </summary>
        /// <param name="parameters">Name/Value list of parameters</param>
        /// <param name="key">Name of parameter to check.</param>
        /// <returns>Boolean value of parameter (false if value is false or not bool)</returns>
        internal static bool FlagTrue(IDictionary<string, string> parameters, string key)
        {
            bool flag;

            if (!bool.TryParse(parameters[key], out flag))
                flag = false;

            return flag;
        }

        /// <summary>
        /// Sets parameter, but doen't treat as a query parameter
        /// </summary>
        /// <example>
        /// //
        /// // Notice how we need UserID as a parameter - we use this pattern a lot.
        /// //
        /// 
        /// SetRequredSegmentParam(parameters, nameof(UserID), val => UserID = val);
        /// 
        /// var req = new Request($"{BaseUrl}users/{UserID}/owned_lists");
        /// 
        /// </example>
        /// <param name="parameters">list of parameters</param>
        /// <param name="paramName">name of parameter containing value to set</param>
        /// <param name="setter">lambda to set property with value</param>
        internal static void SetSegment(
            Dictionary<string, string> parameters,
            string paramName,
            Action<string> setter)
        {
            if (parameters.ContainsKey(paramName))
                setter(parameters[paramName]);
            else
                throw new ArgumentException($"{paramName} is required", paramName);
        }
    }
}
