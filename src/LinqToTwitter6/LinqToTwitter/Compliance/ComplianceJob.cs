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
        [JsonPropertyName("job_name")]
        public string? JobName { get; set; }

        /// <summary>
        /// URL where to download job results
        /// </summary>
        [JsonPropertyName("download_url")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Date/time when results are no longer available
        /// </summary>
        [JsonPropertyName("download_expires_at")]
        public DateTime DownloadExpiresAt { get; set; }

        /// <summary>
        /// Job status
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// URL where to upload tweet IDs
        /// </summary>
        [JsonPropertyName("upload_url")]
        public string? UploadUrl { get; set; }

        /// <summary>
        /// When the job will be reading
        /// </summary>
        [JsonPropertyName("upload_expires_at")]
        public DateTime UploadExpiresAt { get; set; }
    }
}
