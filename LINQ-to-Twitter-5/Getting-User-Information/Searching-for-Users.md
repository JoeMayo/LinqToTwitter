#### Searching for Users

Find users with a search query.

*Entity:* [[User|User Entity]]
*Type:* UserType.Search

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of results to return for each page; max is 20 | int | no |
| IncludeEntities | Include Twitter entities | bool | no |
| Page | Page to return; defaults to 1 | int | no |
| Query | Search query | string | yes |
| tweetMode | Set to TweetMode.Extended for 280 characters returned in Status.FullText. | TweetMode | no |

##### v3.0 Example:

```c#
            var foundUsers =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Search &&
                       user.Query == "JoeMayo"
                 select user)
                .ToListAsync();

            if (foundUsers != null)
                foundUsers.ForEach(user => 
                    Console.WriteLine("User: " + user.ScreenNameResponse));
```

##### v2.1 Example:

```c#
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Search &&
                       usr.Query == "Joe Mayo"
                 select usr)
                 .FirstOrDefault();

            Console.WriteLine("User Name: " + user.Name);
```

*Twitter API:* [users/search](https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-users-search)