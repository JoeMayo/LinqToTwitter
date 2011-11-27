using System;

namespace LinqToTwitter
{
    public class Media
    {
        /// <summary>
        /// Media contents in bytes
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Media name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Mime content type
        /// </summary>
        public MediaContentType ContentType { get; set; }
    }
}
