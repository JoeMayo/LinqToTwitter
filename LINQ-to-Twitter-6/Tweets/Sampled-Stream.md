### Sampled Stream

Return around 1% of all tweets.

*Type:* StreamingType.Sample

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| MediaFields | Comma-separated list of fields to return in the media object | string (MediaField) | no |
| PlaceFields | Comma-separated list of fields to return in the place object | string (PlaceField) | no |
| PollFields | Comma-separated list of fields to return in the poll object | string (PollField) | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

*Return Type:* StreamContent.

##### Example:

```c#
static async Task DoSampleStreamAsync(TwitterContext twitterCtx)
{
	Console.WriteLine("\nStreamed Content: \n");
	int count = 0;
	var cancelTokenSrc = new CancellationTokenSource();

	try
	{
		await
			(from strm in twitterCtx.Streaming
									.WithCancellation(cancelTokenSrc.Token)
			 where strm.Type == StreamingType.Sample
			 select strm)
			.StartAsync(async strm =>
			{
				await HandleStreamResponse(strm);

				if (count++ >= 10)
					cancelTokenSrc.Cancel();
			});
	}
	catch (IOException ex)
	{
		// Twitter might have closed the stream,
		// which they do sometimes. You should
		// restart the stream, but be sure to
		// read Twitter documentation on stream
		// back-off strategies to prevent your
		// app from being blocked.
		Console.WriteLine(ex.ToString());
	}
	catch (OperationCanceledException)
	{
		Console.WriteLine("Stream cancelled.");
	}
}

static async Task<int> HandleStreamResponse(StreamContent strm)
{
	if (strm.HasError)
	{
		Console.WriteLine($"Error during streaming: {strm.ErrorMessage}");
	}
	else
	{
		Tweet? tweet = strm?.Entity?.Tweet;
		if (tweet != null)
			Console.WriteLine($"\nTweet ID: {tweet.ID}, Tweet Text: {tweet.Text}");
	}

	return await Task.FromResult(0);
}
```

*Twitter API:* [tweets/sample/stream](https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/api-reference/get-tweets-sample-stream)