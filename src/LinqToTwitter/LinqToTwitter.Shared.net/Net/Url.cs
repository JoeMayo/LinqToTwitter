using System;
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
            const string ReservedChars = @"`!@#$^&*()+=,:;'?/|\[] ";

            var result = new StringBuilder();

            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var escapedValue = Uri.EscapeDataString(value);

            // Windows Phone doesn't escape all the ReservedChars properly, so we have to do it manually.
            foreach (char symbol in escapedValue)
            {
                if (ReservedChars.IndexOf(symbol) != -1)
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol).ToUpper());
                }
                else
                {
                    result.Append(symbol);
                }
            }

            return result.ToString();
        }
    }
}
