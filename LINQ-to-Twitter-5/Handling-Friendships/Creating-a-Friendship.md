#### Creating a Friendship

Follow another user.

##### Signature:

```c#
public async Task<User> CreateFriendshipAsync(
    ulong userID, bool follow)
public async Task<User> CreateFriendshipAsync(
    string screenName, bool follow)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| follow | Receive notifications about followed user | bool | no |
| screenName | Screen name of user to follow | string | yes |
| userID | ID of user to follow | ulong | yes |

*Return Type:* [User](../LINQ-to-Twitter-Entities/User-Entity.md)

##### v3.0 Example:

```c#
            var user = await twitterCtx.CreateFriendshipAsync("JoeMayo", true);

            if (user != null && user.Status != null)
                Console.WriteLine(
                    "User Name: {0}, Status: {1}",
                    user.Name,
                    user.Status.Text);
```

##### v2.1 Example:

```c#
var user = twitterCtx.CreateFriendship("JoeMayo", null, true);

Console.WriteLine(
    "User Name: {0}, Status: {1}",
    user.Name,
    user.Status.Text);
```

*Twitter API:* [friendships/create](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-create)