using LinqToTwitter.Common;
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToTwitter
{
    public class MediaError
    {
        public MediaError() { }
        public MediaError(JsonData error)
        {
            Code = error.GetValue<int>("code");
            Name = error.GetValue<string>("name");
            Message = error.GetValue<string>("message");
        }

        /// <summary>
        /// Code number from Twitter
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Name of the error
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of why the error occurred
        /// </summary>
        public string Message { get; set; }
    }
}
