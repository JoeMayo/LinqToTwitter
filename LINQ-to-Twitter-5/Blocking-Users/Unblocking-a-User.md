#### Unblocking a User

Unblock a user.

##### Signature:

```c#
public async Task<User> DestroyBlockAsync(
    ulong userID, string screenName, bool skipStatus)
public async Task<User> DestroyBlockAsync(
    ulong userID, string screenName, bool includeEntities, bool skipStatus)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| includeEntities | Set to false to not include entities (default: true) | bool | no |
| screenName | Screen name of user to follow | string | only if UserID is empty |
| skipStatus | Exclude status information | bool | no |
| userID | ID of user to follow | string | only if ScreenName is empty |

*Return Type:* [User](LINQ-to-Twitter-Entities/User-Entity)

##### v3.0 Example:

```c#
            Console.Write("User Screen Name to Unblock: ");
            string userName = Console.ReadLine();

            var user = await twitterCtx.DestroyBlockAsync(0, userName, true);

            if (user != null) 
                Console.WriteLine("User Name: " + user.Name);
```

##### v2.1 Example:

```c#
            Console.Write("User Screen Name to Unblock: ");
            string userName = Console.ReadLine();

            var user = twitterCtx.DestroyBlock(userName);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
```

*Twitter API:* [blocks/destroy](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/post-blocks-destroy)