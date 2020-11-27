#### Hide Replies

Hides/Unhides replies to a tweet.

##### Signature:

```c#
public async Task<bool> HideReplyAsync(string tweetID, CancellationToken cancelToken = default);

public async Task<bool> UnHideReplyAsync(string tweetID, CancellationToken cancelToken = default);
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| tweetID | ID of the replying tweet | string | yes |
| cancelationToken | Optional cancellation token | CancellationToken | no | 

**Return Type:** bool - The hidden status of the tweet: `true` if not hidden or `false` if hidden

**Note:** Will receive 403 Forbidden if tweetID is for a tweet that is not a reply.

##### Example:

```c#
static async Task HideReplyAsync(TwitterContext twitterCtx)
{
	const string TweetID = "1327749647515881473";

	bool isHidden = await twitterCtx.HideReplyAsync(TweetID);

	Console.WriteLine($"Is Hidden: {isHidden}");
}

static async Task UnHideReplyAsync(TwitterContext twitterCtx)
{
	const string TweetID = "1327749647515881473";

	bool isHidden = await twitterCtx.UnHideReplyAsync(TweetID);

	Console.WriteLine($"Is Hidden: {isHidden}");
}
```

*Twitter API:* [tweets/hide-replies/api-reference/put-tweets-id-hidden](https://developer.twitter.com/en/docs/twitter-api/tweets/hide-replies/api-reference/put-tweets-id-hidden)