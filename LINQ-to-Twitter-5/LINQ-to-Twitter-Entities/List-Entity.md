#### List Entity

The list entity contains properties about lists. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Count | Number of statuses per page | int | yes |
| CreatedAt | List creation date | DateTime | no |
| Cursor | Helps page results | string | yes |
| CursorMovement | Cursors for current request | [[Cursors|Cursors Entity]] | no |
| Description | List description | string | yes |
| FilteredToOwnedLists | Only return lists belonging to user | bool | yes |
| Following | Whether user follows list | bool | no |
| FullName | Full list name | string | no |
| ListID | List ID | string | yes |
| ListIDResult | List ID returned from Twitter | string | no |
| MaxID | Max ID to retrieve for statuses | ulong | yes |
| MemberCount | Number of members | int | no |
| Mode | List mode | string | yes |
| Name | Short name of list | string | yes |
| OwnerID | ID of list owner | string | yes |
| OwnerScreenName | Screen name of list owner | string | yes |
| Page | Page number to return | int | no |
| ScreenName | User screen name | string | yes |
| SinceID | Statuses since this ID | string | yes |
| Slug | Catchword for list | string | yes |
| SlugResult | Catchword for list returned from Twitter | string | no |
| Statuses | Statuses for list | List of [[Status|Status Entity]] | no |
| SubscriberCount | Number of subscribers | string | no |
| TrimUser | Truncate all user infor, except for ID | string | yes |
| Type | Type of List query to perform | ListType | no |
| Uri | List URI | string | no |
| UserID | User ID | string | yes |
| Users | Users associated with list | List of [[User|User Entity]] | no |


*Twitter API:* not documented