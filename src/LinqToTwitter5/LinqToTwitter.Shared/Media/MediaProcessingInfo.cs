using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    public class MediaProcessingInfo
    {
        public const string InProgress = "in_progress";
        public const string Failed = "failed";
        public const string Succeeded = "succeeded";

        public MediaProcessingInfo() { }
        public MediaProcessingInfo(JsonData info)
        {
            State = info.GetValue<string>("state");
            CheckAfterSeconds = info.GetValue<int>("check_after_secs");
            ProgressPercent = info.GetValue<int>("progress_percent");
            Error = new MediaError(info.GetValue<JsonData>("error"));
        }

        /// <summary>
        /// Current status of media upload.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Recommended number of seconds to delay between status checks.
        /// </summary>
        public int CheckAfterSeconds { get; set; }

        /// <summary>
        /// Percentage done of upload processing.
        /// </summary>
        public int ProgressPercent { get; set; }

        /// <summary>
        /// If the request failed with won't be null.
        /// </summary>
        public MediaError Error { get; set; }
    }
}
