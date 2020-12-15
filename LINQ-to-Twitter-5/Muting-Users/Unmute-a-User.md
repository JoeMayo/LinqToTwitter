#### Unmute a User

Turn a user's tweets back on.

##### Signature:

```c#
public async Task<User> UnMuteAsync(
    string screenName, CancellationToken cancelToken = default(CancellationToken));
public async Task<User> UnMuteAsync(
    ulong userID, CancellationToken cancelToken = default(CancellationToken))
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| cancelToken | Allows .NET cancellation framework | CancellationToken | no |
| screenName | Screen name of user to mute | string | yes (on matching overload) |
| userID | ID of user to mute | ulong | yes (on matching overload) |

*Return Type:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

##### v3.x Example:

```c#
            const string ScreenName = "JoeMayo";

            User unmutedUser = await twitterCtx.UnMuteAsync(ScreenName);

            Console.WriteLine("You un-muted {0}", unmutedUser.ScreenNameResponse);
```

*Twitter API:* [mutes/users/destroy](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/post-mutes-users-destroy)