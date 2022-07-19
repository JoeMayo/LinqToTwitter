#### Recent Search

Get a count of tweets for the last 7 days.

*Entity:* Counts Entity

*Type:* CountType.Recent

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| EndTime | Date/Time to search to | DateTime | no |
| Granularity | Grouping by "minute", "hour", or "day" | Granularity enum | no |
| Query | Search query | string | yes |
| SinceID | Return tweets whose IDs are greater than this | string | no |
| StartTime | Date/Time to start search | DateTime | no |
| UntilID | Return tweets whose ids are less than this | string | no |

```csharp
string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";

Counts? countsResponse =
	await
	(from count in twitterCtx.Counts
	 where count.Type == CountType.Recent &&
		   count.Query == searchTerm &&
		   count.Granularity == Granularity.Day
	 select count)
	.SingleOrDefaultAsync();

if (countsResponse?.CountRanges != null)
	countsResponse.CountRanges.ForEach(range =>
		Console.WriteLine(
			$"\nStart: {range.Start}" +
			$"\nEnd:   {range.End}" +
			$"\nTweet: {range.TweetCount}"));
else
	Console.WriteLine("No entries found.");
```

*Twitter API:* [tweets/counts/recent](https://developer.twitter.com/en/docs/twitter-api/tweets/counts/api-reference/get-tweets-counts-recent)