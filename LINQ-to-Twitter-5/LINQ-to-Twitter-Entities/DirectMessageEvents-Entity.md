#### DirectMessageEvents Entity

The direct message events entity contains properties about direct message events. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Count | (List Query): Number of items to return for a single page. | int | yes |
| Cursor | (List Query): Supports paging through results on List queries. | string | yes |
| ID | (Show Query): ID of DM. | ulong | yes |
| RecipientID | (New DM): ID of the user the DM is sent to. | ulong | yes |
| Text | (New DM): DM contents. | string | yes |
| Value | Response from Twitter. | [[DirectMessageEventsValue]] | yes |

*Twitter API:* [direct_messages/events](https://developer.twitter.com/en/docs/direct-messages/sending-and-receiving/api-reference)