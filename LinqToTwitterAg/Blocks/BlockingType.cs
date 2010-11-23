using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public enum BlockingType
    {
        /// <summary>
        /// Learn if specific user is being blocked
        /// </summary>
        Exists,

        /// <summary>
        /// Retrieve list of users (full User objects) being blocked
        /// </summary>
        Blocking,

        /// <summary>
        /// Retrieve a list of IDs of users being blocked
        /// </summary>
        IDS
    }
}
