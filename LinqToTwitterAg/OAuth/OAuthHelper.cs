using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace LinqToTwitter
{
    /// <summary>
    /// Methods to facilitate testing and other utilities
    /// </summary>
    public class OAuthHelper : IOAuthHelper
    {
        /// <summary>
        /// Encapsulates GetResponse so tests don't invoke the request
        /// </summary>
        /// <param name="req">Request to Twitter</param>
        /// <returns>Response to Twitter</returns>
        public HttpWebResponse GetResponse(HttpWebRequest req)
        {
            Exception asyncException = null;

            var resetEvent = new ManualResetEvent(initialState: false);
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
    }
}
