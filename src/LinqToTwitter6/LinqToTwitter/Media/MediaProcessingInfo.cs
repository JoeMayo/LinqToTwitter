using LinqToTwitter.Common;
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
            State = info.GetString("state");
            CheckAfterSeconds = info.GetInt("check_after_secs");
            ProgressPercent = info.GetInt("progress_percent");
            info.TryGetProperty("error", out JsonElement errorValue);
            Error = new MediaError(errorValue);
        }

        /// <summary>
        /// Current status of media upload.
        /// </summary>
        public string? State { get; set; }

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
        public MediaError? Error { get; set; }
    }
}
