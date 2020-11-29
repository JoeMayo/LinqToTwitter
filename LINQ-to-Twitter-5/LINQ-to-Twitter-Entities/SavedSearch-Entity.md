#### SavedSearch Entity

The saved search entity contains properties about saved searches. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| CreatedAt | When search was created | DateTime | no |
| ID | Saved search ID | string | yes |
| IDString | Saved search ID as string | string | no |
| Name | Name of search | string | no |
| Position | Position in search list | int | no |
| Query | Search query | string | yes |

*Twitter API:* not documented