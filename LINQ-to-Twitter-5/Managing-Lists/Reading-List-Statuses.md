#### Reading List Statuses

Read tweets from a list.

*Entity:* [List](../LINQ-to-Twitter-Entities/List-Entity.md)

*Type:* ListType.Statuses

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of statuses per page | int | no |
| IncludeEntities | Add entities to tweets (default: true) | bool | no |
| IncludeRetweets | Add retweets, in addition to normal tweets | bool | no |
| ListID | ID of list | ulong | only if Slug is empty |
| MaxID | Max ID to retrieve for statuses | ulong | no |
| OwnerID | ID of user who owns list | ulong | only if Slug is provided |
| OwnerScreenName | Name of user to who owns list | string | only is Slug is provided |
| Page | Page number for statuses | int | no |
| SinceID | Statuses since status ID | ulong | no |
| Slug | Short list name | string | only if ListID is empty |
| TrimUser | Truncate all user info from result, except for ID | bool | no |

Note: Either ListID or Slug is required. If you use Slug, you must also specify either OwnerID or OwnerScreenName.

##### v3.0 Example:

```c#
            string ownerScreenName = "Linq2Tweeter";
            string slug = "linq";
            int maxStatuses = 30;
            int lastStatusCount = 0;
            // last tweet processed on previous query
            ulong sinceID = 204251866668871681; 
            ulong maxID;
            int count = 10;
            var statusList = new List<Status>();

            // only count
            var listResponse =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Statuses &&
                       list.OwnerScreenName == ownerScreenName &&
                       list.Slug == slug &&
                       list.Count == count
                 select list)
                .SingleOrDefaultAsync();

            if (listResponse != null && listResponse.Statuses != null)
            {
                List<Status> newStatuses = listResponse.Statuses;
                // first tweet processed on current query
                maxID = newStatuses.Min(status => status.StatusID) - 1; 
                statusList.AddRange(newStatuses);

                do
                {
                    // now add sinceID and maxID
                    listResponse =
                        await
                        (from list in twitterCtx.List
                         where list.Type == ListType.Statuses &&
                               list.OwnerScreenName == ownerScreenName &&
                               list.Slug == slug &&
                               list.Count == count &&
                               list.SinceID == sinceID &&
                               list.MaxID == maxID
                         select list)
                        .SingleOrDefaultAsync();

                    if (listResponse == null)
                        break;

                    newStatuses = listResponse.Statuses;
                    // first tweet processed on current query
                    maxID = newStatuses.Min(status => status.StatusID) - 1; 
                    statusList.AddRange(newStatuses);

                    lastStatusCount = newStatuses.Count;
                }
                while (lastStatusCount != 0 && statusList.Count < maxStatuses);

                for (int i = 0; i < statusList.Count; i++)
                {
                    Status status = statusList[i];

                    Console.WriteLine("{0, 4}. [{1}] User: {2}\nStatus: {3}",
                        i + 1, status.StatusID, status.User.Name, status.Text);
                }
            }
```

##### v2.1 Example:

```c#
            var statusList =
                (from list in twitterCtx.List
                 where list.Type == ListType.Statuses &&
                       list.OwnerScreenName == "JoeMayo" &&
                       list.Slug == "dotnettwittterdevs" // name of list to get statuses for
                 select list)
                 .First();

            foreach (var status in statusList.Statuses)
            {
                Console.WriteLine("User: {0}, Status: {1}",
                    status.User.Name, status.Text);
            }
```

*Twitter API:* [lists/statuses](https://developer.twitter.com/en/docs/accounts-and-users/create-manage-lists/api-reference/get-lists-statuses)