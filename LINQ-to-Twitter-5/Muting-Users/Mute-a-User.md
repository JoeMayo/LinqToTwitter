#### Mute a User

Turn off user's tweets without unfollowing them.

##### Signature:

```c#
public async Task<User> MuteAsync(
    string screenName, CancellationToken cancelToken = default(CancellationToken));
public async Task<User> MuteAsync(
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
            const string ScreenName = "justinbieber";

            User mutedUser = await twitterCtx.MuteAsync(ScreenName);

            Console.WriteLine("You muted {0}", mutedUser.ScreenNameResponse);
```

*Twitter API:* [mutes/users/create](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/post-mutes-users-create)