#### Getting Muted Users

Retrieve a list of muted user entities.

*Entity:* [Mute](../LINQ-to-Twitter-Entities/Mute-Entity.md)

*Type:* MuteType.List

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Allows you to page through query results | long | no |
| IncludeEntitites | Set to true for Twitter to return entity metadata with users last tweet | bool | no |
| SkipStatus | Set to true to remove tweet from user entities | bool | no |

##### v3.x Example:

```c#
        var muteResponse =
            await
            (from mute in twitterCtx.Mute
             where mute.Type == MuteType.List
             select mute)
            .SingleOrDefaultAsync();

        muteResponse.Users.ForEach(
            user => Console.WriteLine(user.ScreenNameResponse));
```

*Twitter API:* [mutes/users/list](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/get-mutes-users-list)