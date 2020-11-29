#### Favorites Entity

The favorites entity contains properties about favorites. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Count | Number of results | int | yes |
| MaxID | Results no higher than this ID | ulong | yes |
| Page | Page to retrieve | string | yes |
| ScreenName | Screen name to search | string | yes |
| SinceID | Results after this ID | ulong | yes |
| Type | Type of favorites | FavoritesType | yes |
| UserID | User ID to search | string | yes |

*Twitter API:* not documented