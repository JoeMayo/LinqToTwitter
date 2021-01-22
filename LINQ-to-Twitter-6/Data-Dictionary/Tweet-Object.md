#### Tweet Object

Represents a user's tweet.

##### Properties:

| Name | Purpose | Type |
|------|---------|------|
| Attachments | Attachments, such as polls, images, and videos | [TweetAttachment](TweetAttachment-Object.md) |
| AuthorID | Posting user's ID | string |
| ContextAnnotations | Contextual metadata associated with tweet | List&lt;[TweetContextAnnotation](TweetContextAnnotation-Object.md)&gt; |
| ConversationID | ID of conversation this tweet is part of (matches original tweet ID) | string |
| CreatedAt | When tweeted | DateTime |
| Entities | Extracted parts of the tweet, like hashtags, urls, etc. | [TweetEntities](TweetEntities-Object.md) |
| Geo | Tweet location (if user provided) | TweetGeo |
| ID | ID of this tweet | string |
| InReplyToUserID | If replying, this is the ID of the user being replied to | string |
| Language | [BCP47 Language tag](https://tools.ietf.org/html/bcp47) | string |
| NonPublicMetrics | Non-Public Metrics | object |
| OrganicMetrics | Organic Metrics | object |
| PossiblySensitive | Media or links might reveal sensitive information | bool |
| PromotedMetrics | Promoted content metrics | object |
| PublicMetrics | Public metrics | [TweetPublicMetrics]() |
| ReferencedTweets | Any other tweets that this one refers to | List&lt;[TweetReference]()&gt; |
| ReplySettings | Who can reply to tweet | [TweetReplySettings]() |
| Source | The application sending the tweet | string |
| Text | User's tweet text | string |
| Withheld | Information regarding a request to withhold information | [TwitterWithheld]() |

*Twitter API:* [Tweet Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/tweet)