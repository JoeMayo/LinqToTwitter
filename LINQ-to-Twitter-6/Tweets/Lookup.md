#### Querying Statuses

Get information on one or more tweets.

*Entity:* Tweet|Tweet Entity

*Type:* StatusType.Lookup

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| Ids | Up to 100 comma-separated IDs to search for | string | yes |
| MediaFields | Comma-separated list of fields to return in the media object | string (MediaField) | no |
| PlaceFields | Comma-separated list of fields to return in the place object | string (PlaceField) | no |
| PollFields | Comma-separated list of fields to return in the poll object | string (PollField) | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

```c#
const string TweetID = "1305895383260782593";

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

TweetQuery? tweetResponse =
	await
	(from tweet in twitterCtx.Tweets
	 where tweet.Type == TweetType.Lookup &&
		   tweet.Ids == TweetID &&
		   tweet.TweetFields == TweetField.AllFieldsExceptPermissioned &&
		   tweet.Expansions == ExpansionField.AllTweetFields &&
		   tweet.MediaFields == MediaField.AllFieldsExceptPermissioned &&
		   tweet.PlaceFields == PlaceField.AllFields &&
		   tweet.PollFields == PollField.AllFields &&
		   tweet.UserFields == UserField.AllFields
	 select tweet)
	.SingleOrDefaultAsync();

if (tweetResponse?.Tweets != null)
	tweetResponse.Tweets.ForEach(tweet =>
		Console.WriteLine(
			$"\nUser: {tweet.ID}" +
			$"\nTweet: {tweet.Text}"));
else
	Console.WriteLine("No entries found.");
```

*Twitter API:* [tweets/lookup/api-reference](https://developer.twitter.com/en/docs/twitter-api/tweets/lookup/api-reference)