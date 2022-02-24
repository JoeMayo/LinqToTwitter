using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public record Space
    {
        [JsonPropertyName("creator_id")]
        public string? CreatorID { get; set; }

        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        [JsonPropertyName("invited_user_ids")]
        public List<string>? InvitedUserIds { get; set; }

        [JsonPropertyName("participant_count")]
        public int ParticipantCount { get; set; }

        [JsonPropertyName("is_ticketed")]
        public bool IsTicketed { get; set; }

        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("scheduled_start")]
        public DateTime? ScheduledStart { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }

        [JsonPropertyName("id")]
        public string? ID { get; set; }

        [JsonPropertyName("speaker_ids")]
        public List<string>? SpeakerIds { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("subscriber_count")]
        public int SubscriberCount { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("topic_ids")]
        public List<string>? TopicIds { get; set; }

        [JsonPropertyName("host_ids")]
        public List<string>? HostIds { get; set; }
    }
}
