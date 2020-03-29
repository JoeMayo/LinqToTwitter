using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MSEncoder = LinqToTwitter.Security.Application.Encoder;

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE

#endif

namespace LinqToTwitter
{
    public class BuildUrlHelper
    {
        /// <summary>
        /// makes ID parameter part of the URL
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <param name="url">original url</param>
        /// <returns>transformed URL with ID</returns>
        public static string TransformIDUrl(Dictionary<string, string> parameters, string url)
        {
            return TransformParameterUrl(parameters, "ID", url);
        }

        /// <summary>
        /// makes a parameter part of the URL
        /// </summary>
        /// <param name="parameters">parameter dictionary</param>
        /// <param name="key">key in paramaters dictionary</param>
        /// <param name="url">original url</param>
        /// <returns>transformed URL with ID</returns>
        public static string TransformParameterUrl(Dictionary<string, string> parameters, string key, string url)
        {
            if (parameters.ContainsKey(key))
            {
                var fileExtension = Path.GetExtension(url);
                if (fileExtension != null) 
                    url = url.Replace(fileExtension, "/" + parameters[key] + fileExtension);
            }

            return url;
        }


        /// <summary>
        /// Url Encodes a value
        /// </summary>
        /// <param name="value">string to be encoded</param>
        /// <returns>UrlEncoded string</returns>
        public static string UrlEncode(string value)
        {
            const string ReservedChars = @"`!@#$%^&*()_-+=.~,:;'?/|\[] ";
            const string UnReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            var result = new StringBuilder();

            if (string.IsNullOrEmpty(value))
                return string.Empty;

            foreach (var symbol in value)
            {
                if (UnReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else if (ReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol).ToUpper());
                }
                else
                {
                    string symbolString = symbol.ToString();
                    var encoded = MSEncoder.UrlEncode(symbolString, Encoding.UTF8).ToUpper();

                    if (!string.IsNullOrEmpty(encoded))
                    {
                        result.Append(encoded);
                    }
                }
            }

            return result.ToString();
        }

    }
}
