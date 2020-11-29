#### Relationship Entity

The relationship entity contains properties about relationships. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| AllReplies | Whether to show all replies | string | no |
| Blocking | Whether this user is blocking the other | bool | no |
| CanDM | Allowed to send direct messages | bool | no |
| Connections | Relationships between user and other | List of string | no |
| FollowedBy | Whether user is followed by the other | bool | no |
| Following | Whether user is following the other | bool | no |
| ID | User ID | string | no |
| MarkedSpam | Marked as spam | bool | no |
| Name | User name | string | no |
| NotificationsEnabled | Whether notifications are enabled | bool | no |
| RetweetsWanted | Whether user wants retweets from person they follow | bool | no |
| ScreenName | User screen name | string | no |

*Twitter API:* not documented