#### RelatedResults Entity

The related results entity contains properties about related results. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Contributors | List of contributors to tweet | List of [[Contributor|Contributors Entity]] | no |
| Coordinates | Latitude and longitude of tweet | [[Coordinate|Coordinates Entity]] | no |
| CreatedAt | Tweet creation date | DateTime | no |
| Favorited | Whether tweet is favorited | bool | no |
| Geo | Geographical location of result | [[Geo|Geo Entity]] | no |
| IDString | Status ID as string | string | no |
| InReplyToScreenName | Screen name of replier | string | no |
| InReplyToStatusID | Status ID pf replier | ulong | no |
| InReplyToUserID | User ID of replier | ulong | no |
| InReplyToUserIDString | User ID as string of replier | string | no |
| Kind | Kind | string | no |
| Place | Place info from where tweet originated | [[Place|Place Entity]] | no |
| ResultAnnotations | Result annotations | Annotation | no |
| RetweetCount | Number of retweets | int | no |
| Retweeted | Whether tweet has been retweeted | bool | no |
| Score | Score | double | no |
| Source | What software or application produced tweet | string | no |
| StatusID | Tweet ID | ulong | yes |
| Text | Tweet text | string | no |
| Truncated | Whether it is truncated | bool | no |
| Type | Type of result | string | no |
| User | Information on user who tweeteed | [[User|User Entity]] | no |
| ValueAnnotations | Tweet annotations | Annotation | no |

*Twitter API:* not documented