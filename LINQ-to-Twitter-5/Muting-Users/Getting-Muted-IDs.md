#### Getting Muted IDs

Retrieve a list of muted user IDs.

*Entity:* [[Mute|Mute Entity]]
*Type:* MuteType.IDs

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Allows you to page through query results | long | no |

##### v3.x Example:

```c#
            var muteResponse =
                await
                (from mute in twitterCtx.Mute
                 where mute.Type == MuteType.IDs
                 select mute)
                .SingleOrDefaultAsync();

            muteResponse.IDList.ForEach(id => Console.WriteLine(id));
```

*Twitter API:* [mutes/users/ids](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/get-mutes-users-ids)