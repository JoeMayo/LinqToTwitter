### Filtered Stream

Return tweets matching given filters.

*Type:* StreamingType.Filter

##### Parameters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Expansions | Comma-separated list of expansion fields | string (ExpansionField) | no |
| MediaFields | Comma-separated list of fields to return in the media object | string (MediaField) | no |
| PlaceFields | Comma-separated list of fields to return in the place object | string (PlaceField) | no |
| PollFields | Comma-separated list of fields to return in the poll object | string (PollField) | no |
| TweetFields | Comma-separated list of fields to return in the Tweet object | string (TweetField) | no |
| UserFields | Comma-separated list of fields to return in the User object | string (UserField) | no |

*Return Type:* StreamContent.

*Important!:* You need to use `AddStreamingFilterRulesAsync` and `DeleteStreamingFilterRulesAsync` to set/remove stream filters. See `Managing Rules` section below:

##### Example:

```c#
static async Task DoFilterStreamAsync(TwitterContext twitterCtx)
{
	Console.WriteLine("\nStreamed Content: \n");
	int count = 0;
	var cancelTokenSrc = new CancellationTokenSource();

	try
	{
		await
			(from strm in twitterCtx.Streaming
									.WithCancellation(cancelTokenSrc.Token)
			 where strm.Type == StreamingType.Filter
			 select strm)
			.StartAsync(async strm =>
			{
				await HandleStreamResponse(strm);

				if (count++ >= 5)
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

Managing Rules:

```C#
static async Task ValidateRulesAsync(TwitterContext twitterCtx)
{
	var rules = new List<StreamingAddRule>
	{
		new StreamingAddRule { Tag = "memes with media", Value = "meme has:images" },
		new StreamingAddRule { Tag = "cats with media", Value = "cat has:media" }
	};

	Streaming? result = await twitterCtx.AddStreamingFilterRulesAsync(rules, isValidateOnly: true);

	if (result?.Meta?.Summary != null)
	{
		StreamingMeta meta = result.Meta;
		Console.WriteLine($"\nSent: {meta.Sent}");

		StreamingMetaSummary summary = meta.Summary;

		Console.WriteLine($"Created:  {summary.Created}");
		Console.WriteLine($"!Created: {summary.NotCreated}");
	}

	if (result?.Errors != null && result.HasErrors)
		result.Errors.ForEach(error =>
			Console.WriteLine(
				$"\nTitle: {error.Title}" +
				$"\nValue: {error.Value}" +
				$"\nID:    {error.ID}" +
				$"\nType:  {error.Type}"));
}

static async Task AddRulesAsync(TwitterContext twitterCtx)
{
	var rules = new List<StreamingAddRule>
	{
		new StreamingAddRule { Tag = "funny things", Value = "meme" },
		new StreamingAddRule { Tag = "happy cats with media", Value = "cat has:media -grumpy" }
	};

	Streaming? result = await twitterCtx.AddStreamingFilterRulesAsync(rules);

	StreamingMeta? meta = result?.Meta;

	if (meta?.Summary != null)
	{
		Console.WriteLine($"\nSent: {meta.Sent}");

		StreamingMetaSummary summary = meta.Summary;

		Console.WriteLine($"Created:  {summary.Created}");
		Console.WriteLine($"!Created: {summary.NotCreated}");
	}

	if (result?.Errors != null && result.HasErrors)
		result.Errors.ForEach(error => 
			Console.WriteLine(
				$"\nTitle: {error.Title}" +
				$"\nValue: {error.Value}" +
				$"\nID:    {error.ID}" +
				$"\nType:  {error.Type}"));
}

static async Task DeleteRulesAsync(TwitterContext twitterCtx)
{
	var ruleIds = new List<string>
	{
		"1165037377523306498",
		"1165037377523306499"
	};

	Streaming? result = await twitterCtx.DeleteStreamingFilterRulesAsync(ruleIds);

	if (result?.Meta?.Summary != null)
	{
		StreamingMeta meta = result.Meta;
		Console.WriteLine($"\nSent: {meta.Sent}");

		StreamingMetaSummary summary = meta.Summary;

		Console.WriteLine($"Deleted:  {summary.Deleted}");
		Console.WriteLine($"!Deleted: {summary.NotDeleted}");
	}

	if (result?.Errors != null && result.HasErrors)
		result.Errors.ForEach(error =>
			Console.WriteLine(
				$"\nTitle: {error.Title}" +
				$"\nValue: {error.Value}" +
				$"\nID:    {error.ID}" +
				$"\nType:  {error.Type}"));
}
```

*Twitter API:* [tweets/search/stream](https://developer.twitter.com/en/docs/twitter-api/tweets/filtered-stream/api-reference/get-tweets-search-stream)