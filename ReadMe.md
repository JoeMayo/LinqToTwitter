# Notice

As with many parts of life, all good things come to an end. I've decided to move on to other endeavors, which means that this repository is unsupported and no longer under active maintenance. I would like to thank all of the supporters through the years that made LINQ to Twitter a successful project and wish you all the best.

Regards,

[Joe Mayo](https://twitter.com/JoeMayo)

![LINQ to Twitter](https://i.imgur.com/ESNVAR4.png)

LINQ to Twitter is an open source 3rd party LINQ Provider (Twitter Library) for the [Twitter](https://twitter.com/) micro-blogging service.  It uses standard LINQ syntax for queries and includes method calls for changes via the [Twitter API](https://dev.twitter.com/).

[![Standard](https://img.shields.io/endpoint?url=https%3A%2F%2Ftwbadges.glitch.me%2Fbadges%2Fstandard)](https://developer.twitter.com/en/docs/twitter-api)
[![Labs](https://img.shields.io/endpoint?url=https%3A%2F%2Ftwbadges.glitch.me%2Fbadges%2Flabs)](https://developer.twitter.com/en/docs/labs)
[![v2](https://img.shields.io/endpoint?url=https%3A%2F%2Ftwbadges.glitch.me%2Fbadges%2Fv2)](https://developer.twitter.com/en/docs/twitter-api)

## Example

The following query returns search results where people are tweeting about LINQ providers:
```C#
var twitterCtx = new TwitterContext(...);

TwitterSearch? searchResponse =
    await
    (from search in twitterCtx.TwitterSearch
     where search.Type == SearchType.RecentSearch &&
           search.Query == "LINQ to"
     select search)
    .SingleOrDefaultAsync();

if (searchResponse?.Tweets != null)
    searchResponse.Tweets.ForEach(tweet =>
        Console.WriteLine(
            $"\nID: {tweet.ID}" +
            $"\nTweet: {tweet.Text}"));
```
From a coding experience perspective, the `TwitterContext` type is analogous to the Entity Framework `DBContext`.  You use the `TwitterContext` instance, `twitterCtx`, to access `IQueryable<T>` tweet categories.  In the example above, the `TwitterSearch` will give you the ability to search Twitter for tweets meeting some criteria.

> *Note:* The ellipses in the TwitterContext parameters indicates that you need to provide an authorizer with credentials, which is required. You can visit [Securing Your Applications](https://github.com/JoeMayo/LinqToTwitter/wiki/Securing-Your-Applications) for documentation on authorizers and visit the Download page for working examples.

Each query category has a `Type` property for the type of tweets you want to get back.  For example, `Tweet` queries can be made for `Mentions`, `ReverseChronological`, or `Tweets` timelines. Each query category also has an `XxxType` enum to help you figure out what is available. The example above uses `SearchType.RecentSearch` to perform searches on matching tweets that happened within the last two weeks or so.  Another example would be `Like` queries which might have `LikeType.Lookup` as its `Type` to get all the users who liked a tweet.  The `Type` idiom is consistent across all query categories.

Just like other LINQ providers, you get an `IQueryable<T>` back from the query.  You can see how to materialize the query by invoking the `SingleOrDefaultAsync` operator.  For `TwitterSearch` results, you receive one `TwitterSearch` entity that contains metadata about the `Search` query and also contains a `Tweets` property that is a collection of `Tweet` entities. Just like other LINQ providers, LINQ to Twitter does deferred execution, so operators such as `ToListAsync` and `SingleOrDefaultAsync` or statements such as `for` and `foreach` loops will cause the query to execute and make the actual call to Twitter.

LINQ to Twitter is asynchronous. You can see this where the code above `await's` the query, using the `SingleOrDefaultAsync` operator. Commands are async also. e.g. `await TweetAsync("Hello from LINQ to Twitter")`.

For more details on how LINQ to Twitter works, you can visit [LINQ to Twitter v6 APIs](https://www.linqtotwitter.com/LINQ-to-Twitter-v6.html) for API specific examples. The downloadable source code also contains copious examples in the projects. Just look in the _Samples_ folder.

## NuGet
In addition to being able to download from this site, you can also automatically install LINQ to Twitter into your projects via [NuGet](https://www.nuget.org/packages/linqtotwitter); 

## Available Feature Set

See [LINQ to Twitter v6 APIs](https://www.linqtotwitter.com/LINQ-to-Twitter-v6.html).

## For more info:

* follow [@JoeMayo](https://twitter.com/JoeMayo) for releases and related blog posts.
* follow [@Linq2Twitr](https://twitter.com/Linq2Twitr) for more detailed project information.
