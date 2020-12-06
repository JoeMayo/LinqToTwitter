#### Get Blocked IDs

Get a list of IDs of the people a user is blocking

*Entity:* [Blocks](LINQ-to-Twitter-Entities/Blocks-Entity)

*Type:* BlockingType.Ids

##### Parameters/Filters:
| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Separates results into pages | ulong | no |

##### v3.0 Example:

```c#
            var result =
                await
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Ids
                 select blockItem)
                .SingleOrDefaultAsync();

            if (result != null && result.IDs != null)
                result.IDs.ForEach(block => Console.WriteLine("ID: {0}", block)); 
```

##### v2.1 Example:

```c#
            var result =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Ids
                 select blockItem)
                 .SingleOrDefault();

            result.IDs.ForEach(block => Console.WriteLine("ID: {0}", block));
```

Note: Get a single Blocks object and query the IDs collection.

*Twitter API:* [blocks/ids](https://developer.twitter.com/en/docs/accounts-and-users/mute-block-report-users/api-reference/get-blocks-ids)