using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Type of device for account updates
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// no device specified
        /// </summary>
        None,

        /// <summary>
        /// use SMS text
        /// </summary>
        SMS,

        /// <summary>
        /// use IM
        /// </summary>
        IM
    }
}
