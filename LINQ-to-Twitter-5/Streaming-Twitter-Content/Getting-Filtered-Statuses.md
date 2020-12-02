### Getting Filtered Statuses

Return statuses matching given filters.

*Type:* StreamingType.Filter

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Delimited | Tweets are delimited in the stream | string | no |
| Follow | Limit results to comma-separated set of users | string | no |
| Locations | Get tweets in the comma-separate list of lat/lon | string | no |
| Track | Comma-separated list of keywords to get tweets for | string | no |
| StallWarnings | Whether stall warnings should be delivered | bool | no |

*Note:* At least one filter (Follow, Locations, or Track) must be specified.

*Return Type:* JSON string.

##### v3.0 Example:

```c#
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == "twitter"
                 select strm)
                .StartAsync(async strm =>
                {
                    Console.WriteLine(strm.Content + "\n");

                    if (count++ >= 5)
                        strm.CloseStream();
                });
```

##### v2.1 Example:

```c#
            twitterCtx.StreamingUserName = "";
            twitterCtx.StreamingPassword = "";

            if (twitterCtx.StreamingUserName == string.Empty ||
                twitterCtx.StreamingPassword == string.Empty)
            {
                Console.WriteLine("\n*** This won't work until you set the StreamingUserName and StreamingPassword on TwitterContext to valid values.\n");
                return;
            }

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
                where strm.Type == StreamingType.Filter &&
                    strm.Track == "twitter"
                select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status != TwitterErrorStatus.Success)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 2)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
```

*Twitter API:* [statuses/filter](https://developer.twitter.com/en/docs/tweets/filter-realtime/overview/statuses-filter)