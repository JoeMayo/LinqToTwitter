#### Destroying a Favorite

Delete a status from favorites.

##### Signature:

```c#
public async Task<Status> DestroyFavoriteAsync(
    ulong id)
public async Task<Status> DestroyFavoriteAsync(
    ulong id, bool includeEntities)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| id | ID of tweet to remove from favorites | string | yes |
| includeEntities | Include Twitter entities (default: true) | bool | no |

*Return Type:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

##### v3.0 Example:

```c#
            var status = 
                await twitterCtx.DestroyFavoriteAsync(
                    401033367283453953ul, true);

            if (status != null)
                Console.WriteLine(
                    "User: {0}, Tweet: {1}", status.User.Name, status.Text);
```

##### v2.1 Example:

```c#
            var status = twitterCtx.DestroyFavorite("265675496581373952");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
```

*Twitter API:* [favorites/destroy](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-favorites-destroy)