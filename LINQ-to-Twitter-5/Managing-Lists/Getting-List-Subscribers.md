#### Getting List Subscribers

Get a list of users who subscribed to a list.

*Entity:* [[List|List Entity]]
*Type:* ListType.Subscribers

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Cursor | Token to help page through lists | long | no |
| IncludeEntities | Add entities to tweets (default: true) | bool | no |
| ListID | ID of list | ulong | only is Slug is empty |
| OwnerID | ID of list owner | ulong | only if Slug is provided |
| OwnerScreenName | Name of user to who owns list | string | only if Slug is provided |
| ScreenName | ScreenName of list owner | string | only if UserID is empty |
| SkipStatus | Exclude status information | bool | no |
| Slug | Short list name | string | only if ListID is empty |
| UserID | User ID of list owner | ulong | only if ScreenName is empty |

Note: Either UserID or ScreenName is required. Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName. 

##### v3.0 Example:

```c#
            var subscriberList =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Subscribers &&
                       list.Slug == "linq" &&
                       list.OwnerScreenName == "Linq2Tweeter"
                 select list)
                .SingleOrDefaultAsync();

            if (subscriberList != null && subscriberList.Users != null)
                subscriberList.Users.ForEach(user =>
                    Console.WriteLine("Subscriber: " + user.Name));
```

##### v2.1 Example:

```c#
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Subscribers &&
                       list.Slug == "dotnettwittterdevs" &&
                       list.OwnerScreenName == "JoeMayo"
                 select list)
                 .First();

            foreach (var user in lists.Users)
            {
                Console.WriteLine("Subscriber: " + user.Name);
            }
```

*Twitter API:* [lists/subscribers](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-subscribers)