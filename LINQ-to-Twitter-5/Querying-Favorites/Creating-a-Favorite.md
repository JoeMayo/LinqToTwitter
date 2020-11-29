#### Creating a Favorite

Favorite a status.

##### Signature:

```c#
public async Task<Status> CreateFavoriteAsync(
    ulong id)
public async Task<Status> CreateFavoriteAsync(
    ulong id, bool includeEntities)
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| id | ID of tweet to add to favorites | ulong | yes |
| includeEntities | Include Twitter entities (default: true) | bool | no |

*Return Type:* [[Status|Status Entity]]

##### v3.0 Example:

```c#
            var status = await twitterCtx.CreateFavoriteAsync(401033367283453953ul);

            if (status != null)
                Console.WriteLine(
                    "User: {0}, Tweet: {1}", status.User.Name, status.Text);
```

##### v2.1 Example:

```c#
            var status = twitterCtx.CreateFavorite("265675496581373952");

            Console.WriteLine(
                "User: {0}, Tweet: {1}", 
                status.User.Name, status.Text);
```

*Twitter API:* [favorites/create](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-favorites-create)