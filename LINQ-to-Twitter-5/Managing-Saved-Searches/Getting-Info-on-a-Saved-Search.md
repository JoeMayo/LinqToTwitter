#### Getting Info on a Saved Search

Get details on a saved search.

*Entity:* [[SavedSearch|SavedSearch Entity]]
*Type:* SavedSearchType.Show

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Saved search ID | ulong | yes |

##### v3.0 Example:

```c#
            ulong savedSearchID = 306668698;

            var savedSearch =
                await
                (from search in twitterCtx.SavedSearch
                 where search.Type == SavedSearchType.Show &&
                       search.ID == savedSearchID
                 select search)
                .SingleOrDefaultAsync();

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.ID, savedSearch.Name);
```

##### v2.1 Example:

```c#
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Show &&
                      search.ID == "3275867"
                select search;

            var savedSearch = savedSearches.FirstOrDefault();

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
```

*Twitter API:* [saved_searches/show/:id](https://dev.twitter.com/docs/api/1.1/get/saved_searches/show/%3Aid)