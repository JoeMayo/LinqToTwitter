### Getting Status Samples
Return status samples.

*Type:* StreamingType.Sample

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Delimited | Tweets are delimited in the stream | string | no |
| StallWarnings | Whether stall warnings should be delivered | bool | no |

*Return Type:* JSON string.

##### v3.0 Example:

```c#
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Sample
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
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
                where strm.Type == StreamingType.Sample
                select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
```

*Twitter API:* [statuses/sample](https://developer.twitter.com/en/docs/tweets/sample-realtime/overview/GET_statuse_sample)