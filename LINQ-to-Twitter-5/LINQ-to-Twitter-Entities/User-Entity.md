#### User Entity

The user entity contains properties about user information. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| BannerSizes | List of banner sizes | List of [[BannerSize|BannerSize Entity]] | no |
| Categories | Twitter Suggested Users categories | List of [[Category|Category Entity]] | no |
| CreatedAt | Tweet creation date | string | no |
| ContributorsEnabled | Whether contributors is enabled | bool | no |
| Cursor | Which page to retrieve - Previous or Next | string | no |
| CursorMovement | Contains Previous and Next cursors | [[Cursors|Cursors Entity]] | no |
| DefaultProfile | User's profile has not been configured | bool | no |
| DefaultProfileImage | Whether it is default placeholder image | bool | no |
| Description | User description | string | no |
| Email | Email address | string | no |
| FavoritesCount | Number of favorites | int | no |
| FollowersCount | Number of followers | int | no |
| Following | Whether authenticated user is following this user | bool | no |
| FollowRequestSent | Whether authenticated user has sent request to follow this user | bool | no |
| FriendsCount | Number of friends | int | no |
| GeoEnabled | Whether geo track is supported | bool | no |
| ID | User's Twitter ID | string | yes |
| Identifier | Identity properties of specific user | [[UserIdentifier|UserIdentifier Entity]] | no |
| Lang | Display language | string | yes |
| LangResponse | Language to return results as | string | no |
| ListedCount | Number of lists user is a member of | int | no |
| Location | User location | string | no |
| Name | Name of user | string | no |
| Notifications | Type of device notifications | bool | no |
| Page | Page to retrieve | int | yes |
| PerPage | Number of users per page | int | yes |
| ProfileBackgroundColor | Background color of profile | string | no |
| ProfileBackgroundImageUrl | Background image URL of profile | string | no |
| ProfileBackgroundImageUrlHttps | Background profile image used on HTTPS pages | string | no |
| ProfileBackgroundTile | Whether background image tiles | bool | no |
| ProfileImageUrl | User image URL | string | no |
| ProfileImageUrlHttps | Image used on HTTPS pages | string | no |
| ProfileLinkColor | Color of profile links | string | no |
| ProfileSidebarBorderColor | Border color of sidebar | string | no |
| ProfileSidebarFillColor | Background color of profile sidebar | string | no |
| ProfileTextColor | Color of profile text | string | no |
| ProfileUseBackgroundImage | Whether background image is used | bool | no |
| Protected | Whether user is protected | bool | no |
| Query | Search query | string | yes |
| ScreenName | Screen name | string | yes |
| ShowAllInlineMedia | Whether it is enabled | bool | no |
| SkipStatus | Skip status information | bool | yes |
| Slug | Short list name used to identify suggested users category | string | yes |
| Status | User status | string | no |
| StatusCount | Number of user statuses | int | no |
| Url | User URL | string | no |
| TimeZone | Time zone | string | no |
| Translator | Whether user is a translator | bool | no |
| Type | Type of user request - Friends, Followers, Show | UserType | no |
| UserID | User ID | string | yes |
| UtcOffset | UTC offset | int | no |
| Verified | Verified account | bool | no |

*Twitter API:* [users](https://dev.twitter.com/docs/platform-objects/users)