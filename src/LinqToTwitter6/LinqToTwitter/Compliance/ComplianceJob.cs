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
        [JsonPropertyName("id")]
        public string? ID { get; set; }

        /// <summary>
        /// Date/time when job was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Name of the job
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

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

        /// <summary>
        /// Type of job (tweets or users)
        /// </summary>
        [JsonPropertyName("type")]
        public string? JobType { get; set; }

        /// <summary>
        /// Job can be resumed
        /// </summary>
        [JsonPropertyName("resumable")]
        public bool Resumable { get; set; }
    }
}
