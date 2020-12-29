#### TweetMedia Object

Represents a GIF, photo, or video included with a tweet.

##### Properties:

| Name | Purpose | Type |
|------|---------|------|
| DurationMS | Milliseconds duration for videos | int |
| Height | Height in pixels | int |
| MediaKey | Media ID - Matches MediaKey in TweetAttachments | string |
| NonPublicMetrics | Non-Public Metrics | object |
| OrganicMetrics | Organic Metrics | object |
| PreviewImageUrl | URL to video preview image | string |
| PromotedMetrics | Promoted Metrics | object |
| PublicMetrics | Public Metrics | object |
| Type | Type of media - e.g. gif, photo, or video | [TweetMediaType]() |
| Width | Width in pixels | int |

*Twitter API:* [Media Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/media)