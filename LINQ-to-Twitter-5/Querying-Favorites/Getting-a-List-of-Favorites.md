#### Getting a List of Favorites

Get a list of the last 20 favorites for a user.

*Entity:* [Favorites](../LINQ-to-Twitter-Entities/Favorites-Entity.md)

*Type:* FavoritesTypes.Favorites

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of tweets to return; max is 200 | int | no |
| IncludeEntities | Include Twitter entities | bool | 
| MaxID | Return tweets earlier than or equal to this ID | ulong | no |
| ScreenName | Screen name of user | string | only if UserID is empty |
| SinceID | Return tweets later than this ID | ulong | no |
| UserID | ID of user | string | only if ScreenName is empty |

##### v3.0 Example:

```c#
            var favsResponse =
                await
                    (from fav in twitterCtx.Favorites
                     where fav.Type == FavoritesType.Favorites
                     select fav)
                    .ToListAsync();

            if (favsResponse != null)
                favsResponse.ForEach(fav => 
                {
                    if (fav != null && fav.User != null)
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}",
                            fav.User.ScreenNameResponse, fav.Text);
                });
```

##### v2.1 Example:

```c#
            var favorites =
                (from fav in twitterCtx.Favorites
                 where fav.Type == FavoritesType.Favorites &&
                       fav.IncludeEntities == true
                 select fav)
                .ToList();

            foreach (var fav in favorites)
            {
                Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    fav.User.Name, fav.Text);
            }
```

*Twitter API:* [favorites/list](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-favorites-create)