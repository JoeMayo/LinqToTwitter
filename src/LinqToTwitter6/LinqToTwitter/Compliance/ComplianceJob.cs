using System;
using System.Text.Json.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Compliance Job details
    /// </summary>
    public record ComplianceJob
    {
        /// <summary>
        /// Unique ID for job
        /// </summary>
        [JsonPropertyName("job_id")]
        public string? JobID { get; set; }

        /// <summary>
        /// Name of the job
        /// </summary>
        [JsonPropertyName("job_)name")]
        public string? JobName { get; set; }

        /// <summary>
        /// URL where to download job data
        /// </summary>
        [JsonPropertyName("download_url")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Job expiration date/time
        /// </summary>
        [JsonPropertyName("download_expires_at")]
        public DateTime DownloadExpiresAt { get; set; }

        /// <summary>
        /// Job status
        /// </summary>
        [JsonPropertyName("complete")]
        public ComplianceStatus? Status { get; set; }
    }
}
