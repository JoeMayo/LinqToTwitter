#### Getting a List of Saved Searches

Get a list of saved searches.

*Entity:* [SavedSearch](../LINQ-to-Twitter-Entities/SavedSearch-Entity.md)

*Type:* SavedSearchType.Searches

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var savedSearches =
                await
                    (from search in twitterCtx.SavedSearch
                     where search.Type == SavedSearchType.Searches
                     select search)
                    .ToListAsync();

            if (savedSearches != null)
                savedSearches.ForEach(
                    search => Console.WriteLine("Search: " + search.Query));
```

##### v2.1 Example:

```c#
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Searches
                select search;

            foreach (var search in savedSearches)
            {
                Console.WriteLine("ID: {0}, Search: {1}", search.ID, search.Name);
            }
```

*Twitter API:* [saved_searches/list](https://dev.twitter.com/docs/api/1.1/get/saved_searches/list)