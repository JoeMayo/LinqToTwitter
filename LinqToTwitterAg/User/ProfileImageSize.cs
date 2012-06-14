using System;

namespace LinqToTwitter
{
    public enum ProfileImageSize
    {
        /// <summary>
        /// 48 x 48 pixels (default)
        /// </summary>
        Normal,

        /// <summary>
        /// 73 x 73 pixels
        /// </summary>
        Bigger,

        /// <summary>
        /// 24 x 24 pixels
        /// </summary>
        Mini,

        /// <summary>
        /// Size of the originally uploaded image
        /// </summary>
        Original
    }
}
