#### SocialGraph Entity

The social graph entity contains properties about social graphs. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Cursor | Indicator for which page to get next | string | yes |
| CursorMovement | Contains Next and Previous cursors | [[Cursors|Cursors Entity]]| no |
| ID | User ID or ScreenName to retrieve lists for | string | no |
| IDs | List of IDs | List of string | no |
| ScreenName | ScreenName to retrieve lists for | string | yes |
| Type | Type of request | SocialGraphType | no |
| UserID | User ID to retrieve lists for | string | yes |

*Twitter API:* not documented