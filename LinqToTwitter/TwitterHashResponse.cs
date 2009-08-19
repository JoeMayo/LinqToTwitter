using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Response for HTTP errors and end response
    /// </summary>
    [Serializable]
    public class TwitterHashResponse
    {
        /// <summary>
        /// URL action from request
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Error { get; set; }
    }
}
