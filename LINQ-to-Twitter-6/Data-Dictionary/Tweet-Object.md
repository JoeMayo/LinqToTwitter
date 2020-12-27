#### Tweet Object

Represents a user's tweet.

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Attachments | Attachments, such as polls, images, and videos | [Attachments]() | yes |
| AuthorID | Posting user's ID | string | no |
| ContextAnnotations | Contextual metadata associated with tweet | List&lt;[TweetContextAnnotation]()&gt; | no |
| ConversationID | ID of conversation this tweet is part of (matches original tweet ID) | string | no |
| CreatedAt | When tweeted | DateTime | no |
| Entities | Extracted parts of the tweet, like hashtags, urls, etc. | [TweetEntities]() | no |
| Geo | Tweet location (if user provided) | TweetGeo | no |
| ID | ID of this tweet | string | no |
| InReplyToUserID | If replying, this is the ID of the user being replied to | string | no |
| Language | [BCP47 Language tag](https://tools.ietf.org/html/bcp47) | string | no |
| NonPublicMetrics | Non-Public Metrics | object | no |
| OrganicMetrics | Organic Metrics | object | no |
| PossiblySensitive | Media or links might reveal sensitive information | bool | no |
| PromotedMetrics | Promoted content metrics | object | no |
| PublicMetrics | Public metrics | [TweetPublicMetrics]() | no |
| ReferencedTweets | Any other tweets that this one refers to | List&lt;[TweetReference]()&gt; | no |
| ReplySettings | Who can reply to tweet | [TweetReplySettings]() | no |
| Source | The application sending the tweet | string | no |
| Text | User's tweet text | string | no |
| Withheld | Information regarding a request to withhold information | [TwitterWithheld]() | no |

*Twitter API:* [Tweet Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/tweet)