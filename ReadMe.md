![LINQ to Twitter](https://github.com/JoeMayo/LinqToTwitter/raw/master/linq2twitter_v3_300x90.png)

LINQ to Twitter is an open source 3rd party LINQ Provider (Twitter Library) for the [Twitter](https://twitter.com/) micro-blogging service.  It uses standard LINQ syntax for queries and includes method calls for changes via the [Twitter API](https://dev.twitter.com/).

## Example

The following query returns search results where people are tweeting about LINQ to Twitter:
```C#
            var twitterCtx = new TwitterContext(...);

            var searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "\"LINQ to Twitter\""
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
                searchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "User: {0}, Tweet: {1}", 
                        tweet.User.ScreenNameResponse,
                        tweet.Text));
```
From a coding experience perspective, the `TwitterContext` type is analogous to the Entity Framework `DBContext`.  You use the `TwitterContext` instance, `twitterCtx`, to access `IQueryable<T>` tweet categories.  In the example above, the `Search` will give you the ability to search Twitter for tweets meeting some criteria.

> *Note:* The ellipses in the TwitterContext parameters indicates that you need to provide an authorizer with credentials, which is required. You can visit [Securing Your Applications](https://github.com/JoeMayo/LinqToTwitter/wiki/Securing-Your-Applications) for documentation on authorizers and visit the Download page for working examples.

Each query category has a `Type` property for the type of tweets you want to get back.  For example, `Status` tweets can be made for `Home`, `Mentions`, or `User` timelines. Each query category also has an `XxxType` enum to help you figure out what is available. The example above uses `SearchType.Search` to perform searches.  Another example would be `Status` queries which might have `StatusType.Home` as its `Type`.  In the case of `Search` queries, `Search` is the only option, but the `Type` idiom is consistent accross all query categories.

Just like other LINQ providers, you get an `IQueryable<T>` back from the query.  You can see how to materialize the query by invoking the `SingleOrDefaultAsync` operator.  For `Search` results, you receive one `Search` entity that contains information about the `Search` query and the `Search` entity contains a `Statuses` property that is a collection of `Status` entities.  On other queries, you would materialize the query with `ToListAsync` for multiple results.  Just like other LINQ providers, LINQ to Twitter does deferred execution, so operators such as `ToListAsync` and `SingleOrDefaultAsync` or statements such as `for` and `foreach` loops will cause the query to execute and make the actual call to Twitter.

The latest version of LINQ to Twitter supports async. You can see this where the code above `await's` the query, using the `SingleOrDefaultAsync` operator. Commands are async also. e.g. `await TweetAsync("Hello from LINQ to Twitter")`.

For more details on how LINQ to Twitter works, you can visit [Making API Calls](https://github.com/JoeMayo/LinqToTwitter/wiki/Making-API-Calls) for API specific examples.  The downloadable source code also contains copious examples in the projects. Just look in the _Samples_ folder.

## NuGet
In addition to being able to download from this site, you can also automatically install LINQ to Twitter into your Visual Studio projects via [NuGet](https://www.nuget.org/packages/linqtotwitter); 

## Available Feature Set

See [Making API Calls](https://github.com/JoeMayo/LinqToTwitter/wiki/Making-API-Calls).

## For more info:

* follow [@JoeMayo](https://twitter.com/JoeMayo) for releases and related blog posts.
* follow [@Linq2Twitr](https://twitter.com/Linq2Twitr) for more detailed project information.
