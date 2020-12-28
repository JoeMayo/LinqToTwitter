#### TwitterUser Object

Represents a Twitter user.

##### Properties:

| Name | Purpose | Type |
|------|---------|------|
| CreatedAt | Date the user signed up for Twitter | DateTime |
| Description | Profile description | string |
| Entities | Entities associated with this user's profile | [TwitterUserEntities]() |
| ID | User's Twitter ID | string |
| Location | Profile location | string |
| Name | User's entered name in profile | string |
| PinnedTweetID | ID of tweet user has pinned at the top of their timeline |
| ProfileImageUrl | URL to user's profile image | string |
| Protected | Is user account private (access by user's approval only) | bool |
| PublicMetrics | Metrics for this user | [TwitterUserPublicMetrics]() |
| URL | Profile URL | string |
| Username | Twitter assigned screen name | string |
| Verified | Is a Twitter verified account? | bool |

*Twitter API:* [Tweet Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/tweet)