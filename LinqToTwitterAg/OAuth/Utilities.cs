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
using System.Net;
using System.Text;
using System.Threading;

namespace LinqToTwitter
{
    public static class Utilities
    {
        /// <summary>
        /// Encapsulates GetResponse so tests don't invoke the request
        /// </summary>
        /// <param name="req">Request to Twitter</param>
        /// <returns>Response to Twitter</returns>
        public static HttpWebResponse AsyncGetResponse(HttpWebRequest req)
        {
            Exception asyncException = null;

            var resetEvent = new ManualResetEvent(/*initialState:*/ false);
            HttpWebResponse res = null;

            req.BeginGetResponse(
                new AsyncCallback(
                    ar =>
                    {
                        try
                        {
                            res = req.EndGetResponse(ar) as HttpWebResponse;
                        }
                        catch (Exception ex)
                        {
                            asyncException = ex;
                        }
                        finally
                        {
                            resetEvent.Set();
                        }
                    }), null);

            resetEvent.WaitOne();

            if (asyncException != null)
            {
                throw asyncException;
            }

            return res;
        }

        /// <summary>
        /// Reads the web response stream into a string.
        /// </summary>
        /// <param name="resp">The response to read</param>
        /// <returns>a string containing the entire web response body</returns>
        public static string ReadReponse(this WebResponse resp)
        {
            if (resp == null)
                return null;

            using (var respStream = resp.GetResponseStream())
            using (var respReader = new StreamReader(respStream))
            {
                var responseBody = respReader.ReadToEnd();
                return responseBody;
            }
        }

        /// <summary>
        /// Assembles a series of key=value pairs as a URI-escaped query-string.
        /// </summary>
        /// <param name="parameters">The parameters to include.</param>
        /// <returns>A query-string-like value such as a=b&c=d.  Does not include a leading question mark (?).</returns>
        public static string BuildQueryString(IEnumerable<QueryParameter> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            StringBuilder builder = new StringBuilder();
            foreach (var pair in parameters.Where(p => !string.IsNullOrEmpty(p.Value)))
            {
                builder.Append(Uri.EscapeDataString(pair.Name));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(pair.Value));
                builder.Append('&');
            }

            if (builder.Length > 1)
                builder.Length--;   // truncate trailing &

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
