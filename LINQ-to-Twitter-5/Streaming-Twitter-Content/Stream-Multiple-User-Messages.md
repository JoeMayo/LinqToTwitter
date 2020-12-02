### Stream Multiple User Messages
Return multiple users' messages as a stream.

*Type:* UserStreamType.Site

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Delimited | Tweets are delimited in the stream | string | no |
| Follow | Comma-separated list of user IDs to return tweets for | string | no |
| Replies | Return additional replies | string | no |
| StallWarnings | Whether stall warnings should be delivered | bool | no |
| With | Include messages of accounts that the users follow | string | no |

*Return Type:* JSON string.

##### Example:

```c#
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                   //strm.With == "followings" &&
                   strm.Follow == "15411837"//,16761255"
             select strm)
            .StreamingCallback(strm =>
            {
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
```

*Twitter API:* [site](https://dev.twitter.com/docs/api/2b/get/site)