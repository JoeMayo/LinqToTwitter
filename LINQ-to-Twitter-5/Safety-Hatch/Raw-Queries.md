#### Raw Queries

A raw query lets you manually add URL segments and parameters to execute any HTTP GET endpoint in the Twitter API.

*Entity:* [[Raw|Raw Entity]]
*Type:* N/A

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| QueryString | URL segments + query parameters | string | yes |
Notes: 
- Don't include the Base URL in the query string, as it is already set in your TwitterContext instance.
- Url Encode all parameters.

##### Example:
```c#
            string unencodedStatus = "LINQ to Twitter";
            string encodedStatus = Uri.EscapeDataString(unencodedStatus);
            string queryString = "search/tweets.json?q=" + encodedStatus;

            var rawResult =
                await
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == queryString
                 select raw)
                .SingleOrDefaultAsync();

            if (rawResult != null)
                Console.WriteLine(
                    "Response from Twitter: \n\n" + rawResult.Response);
```

*Twitter API:* [Any HTTP GET endpoint in the Twitter API](https://dev.twitter.com/docs/api/1.1)