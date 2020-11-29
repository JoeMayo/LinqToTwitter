#### Status Entity (aka Tweet)

The status entity contains properties about tweets. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Annotation | Meta-data applied to tweet | Annotation | no |
| CreatedAt | Tweet creation date (UTC) | DateTime | no |
| Contributors | List of contributors | List of [[Contributors|Contributors Entity]] | no |
| Coordinates | Coordinate information | [[Coordinates|Coordinates Entity]] | no |
| Count | Number of tweets to return; max is 200 | int | yes |
| CurrentUserRetweet | ID of source status of retweet if IncludeMyRetweet is true. Set to 0 if IncludeMyRetweet is false or tweet not retweeted by authenticating user | int | yes |
| Entities | Entities connected to the status | [[Entities|Entities Entity]] | no |
| ExcludeReplies | Exclude tweet replies | bool | yes |
| FavoriteCount | Number of times the tweet was favorite | int? | no |
| Favorited | Whether the status is Favorited or not | bool | no |
| ID | Tweet ID | string | yes |
| IncludeContributorDetails | Include more than just contributor ID | bool | no |
| InReplyToScreenName | Screen name of user being replied to | string | no |
| InReplyToStatusID | ID of status being replied to | string | no |
| InReplyToUserID | ID of user being replied to | string | no |
| MaxID | Return tweets earlier than or equal to this ID | ulong | yes |
| Page | Page to retrieve | int | yes |
| Place | Place where status was created | [[Place|Place Entity]] | no |
| PossiblySensitive | Whether tweet is possibly sensitive or not | bool | no |
| RetweetCount | Number of times retweeted | int | no |
| Retweeted | Whether status has been retweeted or not | bool | no |
| RetweetedStatus | Whether it was a retweet of a retweeted status | [[Status|Status Entity]] | no |
| ScreenName | Screen name | string | no |
| SinceID | Return tweets later than this ID | ulong | yes |
| Source | Source of the tweet | string | no |
| StatusID | Status ID | string | no |
| Text | Tweet text | string | no |
| TrimUser | Return only user status ID and tweet | bool | yes |
| Truncated | Whether the tweet is truncated or not | bool | no |
| User | User information | [[User|User Entity]] | no |
| Users | List of User | List of [[User|User Entity]] | no |
| UserID | User ID | string | yes |

*Twitter API:* [tweets](https://dev.twitter.com/docs/platform-objects/tweets)
