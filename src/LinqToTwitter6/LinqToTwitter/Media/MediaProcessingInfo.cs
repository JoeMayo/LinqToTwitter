using System.Text.Json;

namespace LinqToTwitter
{
    public class MediaProcessingInfo
    {
        public const string InProgress = "in_progress";
        public const string Failed = "failed";
        public const string Succeeded = "succeeded";

        public MediaProcessingInfo() { }
        public MediaProcessingInfo(JsonElement info)
        {
            State = info.GetProperty("state").GetString();
            CheckAfterSeconds = info.GetProperty("check_after_secs").GetInt32();
            ProgressPercent = info.GetProperty("progress_percent").GetInt32();
            Error = new MediaError(info.GetProperty("error"));
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
