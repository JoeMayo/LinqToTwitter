![LINQ to Twitter](https://github.com/JoeMayo/LinqToTwitter/raw/master/linq2twitter_v3_300x90.png)

LINQ to Twitter is an open source 3rd party LINQ Provider for the [Twitter](https://twitter.com/) micro-blogging service.  It uses standard LINQ syntax for queries and includes method calls for changes via the [Twitter API](https://dev.twitter.com/).

[![Join the chat at https://gitter.im/JoeMayo/LinqToTwitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/JoeMayo/LinqToTwitter?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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
From a coding experience perspective, the _TwitterContext_ type is analogous to _DataContext_ (LINQ to SQL) or _ObjectContext_ (LINQ to Entities).  You use the _TwitterContext_ instance, _twitterCtx_, to access _IQueryable<T>_ tweet categories.  In the example above, the _Search_ will give you the ability to search Twitter for tweets meeting some criteria.

*Note:* _The ellipses in the TwitterContext parameters indicates that you need to provide an authorizer with credentials, which is required. You can visit [Securing Your Applications](https://github.com/JoeMayo/LinqToTwitter/wiki/Securing-Your-Applications) for documentation on authorizers and visit the Download page for working examples._

Each query category has a _Type_ property for the type of tweets you want to get back.  For example, _Status_ tweets can be made for _Home_, _Mentions_, or _User_ timelines. Each query category also has an _XxxType_ enum to help you figure out what is available. The example above uses _SearchType.Search_ to perform searches.  Another example would be _Status_ queries which might have _StatusType.Home_ as its _Type_.  In the case of _Search_ queries, _Search_ is the only option, but the _Type_ idiom is consistent accross all query categories.

Just like other LINQ providers, you get an _IQueryable<T>_ back from the query.  You can see how to materialize the query by invoking the _Single_ operator.  For _Search_ results, you receive one _Search_ entity that contains information about the _Search_ query and the _Search_ entity contains a _Results_ property that is a collection of _SearchResult_ entities.  On other queries, you would materialize the query with _ToList_ for multiple results.  Just like other LINQ providers, LINQ to Twitter does deferred execution, so operators such as _ToList_ and _Single_ or statements such as _for_ and _foreach_ loops will cause the query to execute and make the actual call to Twitter.

The latest version of LINQ to Twitter supports async. You can see this where the code above _await_'s the query, using the _SingleOrDefaultAsync()_ operator. Commands are async also. e.g. _TweetAsync()_.

For more details on how LINQ to Twitter works, you can either click on the _Documentation_ menu (above) or visit [Making API Calls] for API specific examples.  The downloadable source code also contains copious examples in the projects with the _Linq2Twitter__ prefix.

## NuGet
In addition to being able to download from this site, you can also automatically install LINQ to Twitter into your Visual Studio projects via [NuGet](http://bit.ly/mpkwA6); 

For help in getting started with NuGet: [A Gentle Introduction to NuGet](http://bit.ly/iuPkRf).

## Available Feature Set

See [Making API Calls](https://github.com/JoeMayo/LinqToTwitter/wiki/Making-API-Calls).

#### For the latest news, follow [@JoeMayo](https://twitter.com/JoeMayo) on Twitter.
