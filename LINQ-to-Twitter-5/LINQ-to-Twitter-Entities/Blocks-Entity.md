#### Blocks Entity

The blocks entity contains properties about blocks. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| ID | ID or screen name of user | int | yes |
| IDs | List of blocked IDs | List of string | no |
| ListedCount | Listed count | int | no |
| NoBlock | Twitter response for no block | [[TwitterHashResponse|TwitterHashResponse Entity]] | no |
| Page | Page to retrieve | string | yes |
| ScreenName | Screen name to search | string | yes |
| Type | Type of blocks request | BlocksType | no |
| User | Blocked user | [[User|User Entity]] | no |
| UserID | User ID | string | yes |

*Twitter API:* not documented