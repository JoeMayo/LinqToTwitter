#### Recent Search

Search for tweets that occurred within the last week.

*Entity:* TwitterSearch|TwitterSearch Entity

*Type:* SearchType.RecentSearch

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| EndTime | Date/Time to search to | DateTime | no |
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| MaxResults | Maximum number of tweets to return | int | no |
| MediaFields | Comma-separated list of fields to return in the media object | string (MediaField) | no |
| NextToken | Provide this, when paging, to get the next page of results | string | no |
| PlaceFields | Comma-separated list of fields to return in the place object | string (PlaceField) | no |
| PollFields | Comma-separated list of fields to return in the poll object | string (PollField) | no |
| Query | Search query | string | yes |
| SinceID | Return tweets whose IDs are greater than this | string | no |
| StartTime | Date/Time to start search | DateTime | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UntilID | Return tweets whose ids are less than this | string | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

```c#
string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";

// default is id and text and this also brings in created_at and geo
string tweetFields =
	string.Join(",",
		new string[]
		{
			TweetField.CreatedAt,
			TweetField.ID,
			TweetField.Text,
			TweetField.Geo
		});

TwitterSearch? searchResponse =
	await
	(from search in twitterCtx.TwitterSearch
	 where search.Type == SearchType.RecentSearch &&
		   search.Query == searchTerm &&
		   search.TweetFields == TweetField.AllFieldsExceptPermissioned &&
		   search.Expansions == ExpansionField.AllTweetFields &&
		   search.MediaFields == MediaField.AllFieldsExceptPermissioned &&
		   search.PlaceFields == PlaceField.AllFields &&
		   search.PollFields == PollField.AllFields &&
		   search.UserFields == UserField.AllFields
	 select search)
	.SingleOrDefaultAsync();

if (searchResponse?.Tweets != null)
	searchResponse.Tweets.ForEach(tweet =>
		Console.WriteLine(
			$"\nUser: {tweet.ID}" +
			$"\nTweet: {tweet.Text}"));
else
	Console.WriteLine("No entries found.");
```

*Twitter API:* [tweets/search/api-reference/get-tweets-search-recent](https://developer.twitter.com/en/docs/twitter-api/tweets/search/api-reference/get-tweets-search-recent)