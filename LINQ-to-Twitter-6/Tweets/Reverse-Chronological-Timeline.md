#### Reverse Chronological Timeline

Recent tweets of authorized user and those they follow.

*Entity:* Tweet|Tweet Entity

*Type:* TweetType.ReverseChronologicalTimeline

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | ID of the user who's timeline should be read | string | yes |
| EndTime | Date/Time to search to | DateTime | no |
| Exclude | Comma-separated list of tweet types to not include ("retweets" or "replies") | string | no |
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| MaxResults | Maximum number of tweets to return | int | no |
| MediaFields | Comma-separated list of fields to return in the media object | string (MediaField) | no |
| PaginationToken | Provide this, when paging, to get the next or previous page of results | string | no |
| PlaceFields | Comma-separated list of fields to return in the place object | string (PlaceField) | no |
| PollFields | Comma-separated list of fields to return in the poll object | string (PollField) | no |
| SinceID | Return tweets whose IDs are greater than this | string | no |
| StartTime | Date/Time to start search | DateTime | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UntilID | Return tweets whose ids are less than this | string | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

##### Example:

```csharp
string userID = "15411837";

TweetQuery? tweetResponse =
	await
	(from tweet in twitterCtx.Tweets
	 where tweet.Type == TweetType.ReverseChronologicalTimeline &&
		   tweet.ID == userID
	 select tweet)
	.SingleOrDefaultAsync();

if (tweetResponse?.Tweets != null)
	tweetResponse.Tweets.ForEach(tweet =>
		Console.WriteLine(
			$"\nID: {tweet.ID}" +
			$"\nTweet: {tweet.Text}"));
else
	Console.WriteLine("No entries found.");
```

*Twitter API:* [users/:id/timelines/reverse_chronological](https://developer.twitter.com/en/docs/twitter-api/tweets/timelines/api-reference/get-users-id-reverse-chronological)