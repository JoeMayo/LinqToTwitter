//-----------------------------------------------------------------------
// <copyright file="Utilities.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace LinqToTwitter
{
    public static class Utilities
    {
        /// <summary>
        /// Creates a new Uri based on a given Uri, with an appended query string containing all the given parameters.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A new Uri instance.</returns>
        internal static Uri AppendQueryString(Uri requestUri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            if (parameters == null)
            {
                return requestUri;
            }

            UriBuilder builder = new UriBuilder(requestUri);
            if (!string.IsNullOrEmpty(builder.Query))
            {
                builder.Query += "&" + BuildQueryString(parameters);
            }
            else
            {
                builder.Query = BuildQueryString(parameters);
            }

            return builder.Uri;
        }

        /// <summary>
        /// Assembles a series of key=value pairs as a URI-escaped query-string.
        /// </summary>
        /// <param name="parameters">The parameters to include.</param>
        /// <returns>A query-string-like value such as a=b&c=d.  Does not include a leading question mark (?).</returns>
        internal static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            StringBuilder builder = new StringBuilder();
            foreach (var pair in parameters.Where(p => !string.IsNullOrEmpty(p.Value)))
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }

                builder.Append(Uri.EscapeDataString(pair.Key));
                builder.Append("=");
                builder.Append(Uri.EscapeDataString(pair.Value));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Reads a file into a byte array
        /// </summary>
        /// <param name="filePath">Full path of file to read.</param>
        /// <returns>Byte array with file contents.</returns>
        public static byte[] GetFileBytes(string filePath)
        {
            byte[] fileBytes = null;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var memStr = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                memStr.Position = 0;
                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStr.Write(buffer, 0, bytesRead);
                }

                memStr.Position = 0;
                fileBytes = memStr.GetBuffer();
            }

            return fileBytes;
        }
    }
}
