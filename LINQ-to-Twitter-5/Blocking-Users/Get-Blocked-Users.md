#### Get Blocked Users

Find out who a user is blocking.

*Entity:* [[Blocks|Blocks Entity]]
*Type:* BlockingType.Blocking

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Separates results into pages | ulong | no |
| PerPage | Number of results per page | int | no |
| Page | Page to return. Defaults to first page. | int | no |
| SkipStatus | Exclude status information | bool | no |

##### v3.0 Example:

```c#
            var blockResponse =
                await
                    (from block in twitterCtx.Blocks
                     where block.Type == BlockingType.List
                     select block)
                    .SingleOrDefaultAsync();

            if (blockResponse != null && blockResponse.Users != null)
                blockResponse.Users.ForEach(user =>
                        Console.WriteLine(user.ScreenNameResponse)); 
```


##### v2.1 Example:

```c#
            var block =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Blocking
                 select blockItem)
                 .FirstOrDefault();

            block.Users.ForEach(
                user => Console.WriteLine("User, {0} is blocked.", user.Name));
```

Note: Get a single Blocks object and query the Users collection.

*Twitter API:* [blocks/list](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/get-blocks-list)