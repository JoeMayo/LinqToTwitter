#### TweetEntityUrl Object

Entity URL details.

##### Properties:

| Name | Purpose | Type |
|------|---------|------|
| Start | Starting position of URL | int |
| Description | From Twitter Card description (in HTML header) | string |
| DisplayUrl | How URL appears in the tweet | string |
| End | Ending position of URL | int |
| ExpandedUrl | URL that user typed, may or may not be shortened URL | string |
| Images | Details on images attached to tweet | List&lt;[TweetEntityImage](TweetEntityImage-Object.md)&gt; |
| Start | Starting position of URL | int |
| Status | HTTP status from unwind | int |
| Title | From Twitter Card title (in HTML header) | string |
| UnwoundUrl | Final destination URL, after following intermediate shorteners (if any) | string |
| Url | Twitter's t.co URL | string |


*Twitter API:* [Part of Tweet Object](https://developer.twitter.com/en/docs/twitter-api/data-dictionary/object-model/tweet)