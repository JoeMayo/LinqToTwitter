### Stream User Messages
Return a single user's messages as a stream.

*Type:* UserStreamType.User

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Delimited | Tweets are delimited in the stream | string | no |
| Locations | Include tweets within specified bounding box | string | no |
| Replies | Return additional replies | string | no |
| StallWarnings | Whether stall warnings should be delivered | bool | no |
| Track | Include tweets matching comma-separated list of keywords | string | no |

*Return Type:* JSON string.

##### v3.0 Example:

```c#
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.User
                 select strm)
                .StartAsync(async strm =>
                {
                    string message = 
                        string.IsNullOrEmpty(strm.Content) ? 
                            "Keep-Alive" : strm.Content;
                    Console.WriteLine(
                        (count + 1).ToString() + 
                        ". " + DateTime.Now + 
                        ": " + message + "\n");

                    if (count++ == 5)
                        strm.CloseStream();
                });
```

##### v2.1 Example:

```c#
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            // the user stream is for whoever is authenticated
            // via the Authenticator passed to TwitterContext
            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.User
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 25)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
```

*Twitter API:* [ user](https://dev.twitter.com/docs/api/1.1/get/user)