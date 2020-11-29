#### Mute Entity

The Mute entity contains properties supporting the Mute API. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Cursor | Allows you to page through query results | long | yes |
| CursorMovement | Cursors for current request | [[Cursors|Cursors Entity]] | no |
| IDList | List of ids that are muted, populated by IDs query | List<ulong> | no |
| IncludeEntitites | Set to true for Twitter to return entity metadata with users last tweet | bool | yes|
| SkipStatus | Set to true to remove tweet from user entities | bool | yes |
| Users | List of User that are muted, populated by List query | List<[[User|User Entity]]> | no |

*Twitter API:* not documented