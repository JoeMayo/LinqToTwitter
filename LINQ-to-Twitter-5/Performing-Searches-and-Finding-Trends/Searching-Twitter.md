#### Searching Twitter

Perform a search.

*Entity:* [[Search|Search Entity]]
*Type:* SearchType.Search (optional)

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Count | Number of tweets to retrieve for each page. Max is 100. | int | no |
| GeoCode | Tweets within the Radius from a specified Latitude and Longitude. Specify as "latitude,longitude,radius". | string | no |
| IncludeEntities | Omits entities when set to false. Default is true. | bool | no |
| Lang | Language to return tweets in as defined by [ISO-639-1](http://en.wikipedia.org/wiki/ISO_639-1]. | string | no |
| Locale | Language used in query. _ja_ is currently the only supported language other than _en_. Defaults to _en_. | string | no |
| MaxID | Return tweets prior to or equal to this ID. | ulong | no |
| Query | Search query. Can contain search operators defined at [Twitter Search API docs](https://dev.twitter.com/docs/api/1/get/search]. | string | yes |
| ResultType | Specify whether tweets should be Popular, Recent, or Mixed; defaults to recent. | ResultType | no |
| SearchLanguage | Language of tweets. | int | no |
| SinceID | Return tweets later than this ID. | ulong | no |
| Type | Search type. | string | no |
| Until | Tweets up to this date, YYYY-MM-DD. | string | no |
##### v3.0+ Example:

```c#
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";
            //searchTerm = "кот (";

            Search searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.IncludeEntities == true &&
                       search.TweetMode == TweetMode.Extended
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Statuses != null)
                searchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}", 
                        tweet.User.ScreenNameResponse,
                        tweet.User.UserIDResponse,
                        tweet.Text ?? tweet.FullText));
            else
                Console.WriteLine("No entries found.");
```

##### Paging Demo:
```c#
            const int MaxSearchEntriesToReturn = 10;
            const int MaxTotalResults = 100;

            string searchTerm = "twitter";

            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID; 

            var combinedSearchResults = new List<Status>();

            List<Status> searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.Count == MaxSearchEntriesToReturn &&
                       search.SinceID == sinceID &&
                       search.TweetMode == TweetMode.Extended
                 select search.Statuses)
                .SingleOrDefaultAsync();

            if (searchResponse != null)
            {
                combinedSearchResults.AddRange(searchResponse);
                ulong previousMaxID = ulong.MaxValue;
                do
                {
                    // one less than the newest id you've just queried
                    maxID = searchResponse.Min(status => status.StatusID) - 1;

                    Debug.Assert(maxID < previousMaxID);
                    previousMaxID = maxID;

                    searchResponse =
                        await
                        (from search in twitterCtx.Search
                         where search.Type == SearchType.Search &&
                               search.Query == searchTerm &&
                               search.Count == MaxSearchEntriesToReturn &&
                               search.MaxID == maxID &&
                               search.SinceID == sinceID &&
                               search.TweetMode == TweetMode.Extended
                         select search.Statuses)
                        .SingleOrDefaultAsync();

                    combinedSearchResults.AddRange(searchResponse);
                } while (searchResponse.Any() && combinedSearchResults.Count < MaxTotalResults);

                combinedSearchResults.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}",
                        tweet.User.ScreenNameResponse,
                        tweet.User.UserIDResponse,
                        tweet.Text ?? tweet.FullText)); 
            }
            else
            {
                Console.WriteLine("No entries found.");
            }
```

##### v2.1 Example:

```c#
            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter" &&
                       search.Count == 7
                 select search)
                .SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));
```

*Twitter API:* [search/tweets](https://developer.twitter.com/en/docs/tweets/search/api-reference/get-search-tweets)