#### Search Entity

The search entity contains properties about searches. The input column designates whether the property is populated by the user and not Twitter (yes) or if it is a property populated by Twitter (no).

##### Properties:

| Name | Purpose | Type | Input |
|------|---------|------|-------|
| Count | Number of tweets to retrieve for each page. Max is 100. | int | no |
| Geocode | Location specified as "latitude,longitude,radius" | string | yes |
| IncludeEntities | Omits entities when set to false. Default is true. | bool | no |
| Lang | Language to return tweets in as defined by [ISO-639-1](http://en.wikipedia.org/wiki/ISO_639-1). | string | no |
| Locale | Language of search query | string | yes |
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| Query | Search query | string | yes |
| RefreshUrl | Query string to refresh this search | string | no |
| ResultType | Metadata for type of result - mixed, recent, or popular | string | yes |
| SearchLanguage | Returns tweets of specified language | string | yes |
| SearchMetadata | Additional information about query | [[SearchMetaData|SearchMetaData Entity]] | no |
| SinceID | Return tweets later than this ID | ulong | yes |
| SinceIDResult | SinceID returned from Twitter | ulong | no |
| Statuses | Tweet data returned from search | List of [[Status|Status Entity]] | no |
| Type | Type of search | SearchType | yes |
| Until | Return tweets until this date | DateTime | yes |


*Twitter API:* [tweets](https://dev.twitter.com/docs/platform-objects/tweets)
