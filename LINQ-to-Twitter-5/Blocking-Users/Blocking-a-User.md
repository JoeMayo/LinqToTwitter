#### Blocking a User

Block a user.

##### Signature:

```c#
public async Task<User> CreateBlockAsync(
    ulong userID, string screenName, bool skipStatus)
public async Task<User> CreateBlockAsync(
    ulong userID, string screenName, bool includeEntities, bool skipStatus)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| includeEntities | Include Twitter entities (default: true) | bool | no |
| screenName | Screen name of user to follow | string | only if UserID is empty |
| skipStatus | Exclude status information | bool | no |
| userID | ID of user to follow | string | only if ScreenName is empty |

*Return Type:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

##### v3.0 Example:

```c#
            Console.Write("User Screen Name to Block: ");
            string userName = Console.ReadLine();

            var user = await twitterCtx.CreateBlockAsync(0, userName, true);

            if (user != null)
                Console.WriteLine("User Name: " + user.Name);
```


##### v2.1 Example:

```c#
            Console.Write("User Screen Name to Block: ");
            string userName = Console.ReadLine();

            var user = twitterCtx.CreateBlock(userName);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
```

*Twitter API:* [blocks/create](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/post-blocks-create)