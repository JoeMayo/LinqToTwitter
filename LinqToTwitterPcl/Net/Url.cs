using System;
using System.Linq;
using System.Text;

namespace LinqToTwitter.Net
{
    public class Url
    {
        /// <summary>
        /// Implements Percent Encoding according to RFC 3986
        /// </summary>
        /// <param name="value">string to be encoded</param>
        /// <returns>Encoded string</returns>
        public static string PercentEncode(string value)
        {
            const string ReservedChars = @"`!@#$%^&*()_-+=.~,:;'?/|\[] ";
            const string UnReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            var result = new StringBuilder();

            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            foreach (char symbol in value)
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
                    var encoded = Uri.EscapeDataString(symbolString).ToUpper();

                    if (!string.IsNullOrWhiteSpace(encoded))
                    {
                        result.Append(encoded);
                    }
                }
            }

            return result.ToString();
        }
    }
}
