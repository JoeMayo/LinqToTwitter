#### Destroying a Friendship

Unfollow another user.

##### Signature:

```c#
public async Task<User> DestroyFriendshipAsync(ulong userID)
public async Task<User> DestroyFriendshipAsync(string screenName)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| screenName | Screen name of user to follow | string | Yes |
| userID | ID of user to follow | ulong | Yes |

*Return Type:* [[User|User Entity]]

##### v3.0 Example:

```c#
            var user = await twitterCtx.DestroyFriendshipAsync("Linq2Tweeter");

            if (user != null && user.Status != null)
                Console.WriteLine(
                    "User Name: {0}, Status: {1}",
                    user.Name,
                    user.Status.Text);
```

##### v2.1 Example:

```c#
var user = twitterCtx.DestroyFriendship("LinqToTweeter", null, true);

Console.WriteLine(
    "User Name: {0}, Status: {1}",
    user.Name,
    user.Status.Text);
```

*Twitter API:* [friendships/destroy](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-destroy)