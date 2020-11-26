## LINQ to Twitter FAQ

### 1. I get a _401 Unauthorized_ Exception. How to I figure out the problem?

A _401 Unauthorized_ message means that the Twitter server is unable to authorize you.  There are several causes for this problem and the following checklist should help you work through the problems:

* Have you added credentials to an authorizer.  You can obtain these credentials from your registered Twitter application.  You can register a Twitter application at the Twitter developer site at https://developer.twitter.com.  The following documentation explains how to add credentials to a LINQ to Twitter authorizer - [[Securing Your Applications]]
	
* Are your credentials entered properly.  Check for misspellings and potential copy and paste errors. Something such as a trailing space is a common gottcha. You can double-check your credentials at the Twitter developer site (https://developer.twitter.com), explained in the previous item.
	
* Did you assign the right key to the right token.  Make sure you didn't accidentally mix up your `ConsumerKey `and `ConsumerSecret`. You can verify your credentials at the Twitter developer site (https://developer.twitter.com).

* Have your tokens expired? Twitter tokens generally don't expire, but you can always visit your app page and regenerate them if they do.

* If you aren't using `SingleUserAuthorizer`, make sure the authorizing user has authorized your app by going through the authorization process at least one time.
	
* Make sure you've entered a Callback URL for your Twitter application at the Twitter developer site (https://developer.twitter.com).

* Twitter won't let you use http://localhost as your callback URL, so you may want to use http://127.0.0.1 instead.
	
* Check your access level at the Twitter developers site (https://developer.twitter.com). Potential settings are Read, Write, and Direct Message.

* Make sure your access level at the Twitter developers site (https://developer.twitter.com) is set for both Read and Write if you need it.
	
* Ensure your computer time is up-to-date.  The Twitter server reads your OAuth signature (LINQ to Twitter prepares for you) and verifies the time used to build the signature.  This time comes from your machine.  If the time is off, your OAuth signature is considered invalid.  Be aware that the mobile phone emulators time may need to be reset every time the emulator boots up.

* Has Twitter disabled your credentials?  If you post a message on the Twitter forums that exposes your credentials, Twitter will invalidate your credentials to secure your account.  You can re-generate credentials at any time.
	
* Has Twitter revoked your application's access. If you've violated any Twitter rules, they might disable your application.

* Ensure you have permission to perform the operation you're attempting.  i.e. Some features are in Beta and require permission.  Other features, such as XAuth require permission from Twitter to use.

* Examine the API call you're trying to use and make sure you have permission to perform the action you're trying to perform.  Consult the Twitter documentation (linked to from the bottom of each LINQ to Twitter API documentation page) for the latest details of how the API should be used.

* Are you using `ApplicationOnlyAuthorizer` improperly? `ApplicationOnlyAuthorizer` only works for operations that don't operate on behalf of a user. e.g. Search works fine, but `TweetAsync` doesn't because it tweets on behalf of a user.

* Are you using `ApplicationOnlyAuthorizer` on Streams? It doesn't work on streams and you'll need to use a different type of authorizer.

* If using `SingleUserAuthorizer`, the mapping between LINQ to Twitter/Twitter API credentials is `OAuthToken/AccesssToken` and `AccessToken/AccessTokenSecret`. Alternatively, use the `SingleUserInMemoryCredentials` and populate `TwitterAccessToken`and `TwitterAccessTokenSecret` instead of `OAuthToken` and `AccessToken`, respectively.

* When doing an `Account/VerifyCredentials` (see [[Managing Accounts]]) query, Twitter will return a 401 if the user's credentials aren't valid.

* If you're using `XAuthAuthorizer`, you are required to get permission from Twitter before using `XAuth`.

* Review additional troubleshooting tips on Twitter's site at [OAuth FAQ](https://developer.twitter.com/en/docs/basics/authentication/guides/oauth-faq)

* Please remember to sanitize credentials before posting code or HTTP messages in the discussion forums.

* Tip: The problem could also involve a combination of these problems. So, it's useful to keep working each item until the problem is resolved.

### 2. I get a _400 Bad Request_ `WebException`. How to I figure out the problem?

A LINQ to Twitter builds all of the HTTP requests for you, so there's only a limited number of scenarios where this error occurs. In Twitter API v1.0, rate limiting errors appeared as 400, but LINQ to Twitter now supports the Twitter API v1.1, which returns 429 Too Many Requests for rate limit errors.

The most likely reason for a 400 Bad Request is that you didn't authenticate your request. Twitter API v1.1, which LINQ to Twitter supports, requires you to authorize each request. That means you need to use OAuth. For information on how to use OAuth with LINQ to Twitter, visit the [[Securing Your Applications]] page. The downloadable source code on this site has examples that use OAuth too.

As with most errors from Twitter, a good way to debug the problem is to use Fiddler to view the HTTP response. 

* Please remember to sanitize credentials before posting code or HTTP messages in the discussion forums.

### 3. I received an Exception with HTTP Status 429. What does that mean?

This means you've exceeded the rate limit for an Twitter API request. Twitter API has a feature called Rate Limiting, where it only allows you to send a number of requests within a period of time. You can fix this problem by designing your algorithm so that it won't exceed the rate limit.  The Rate Limit varies for each API, so you'll need to design your code accordingly.  LINQ to Twitter exposes various parts of the Twitter API's Rate Limiting features, which includes response headers and queries.  The response headers you can check are available via `TwitterContext` instance properties: `RateLimitCurrent`, `RateLimitRemaining`, and `RateLimitReset`. There are also separate rate limiting properties on the `TwitterContext` entity: `MediaRateLimitCurrent`, `MediaRateLimitRemaining`, and `MediaRateLimitReset`. These header values provide real-time status of rate limits that you can read right after a command or query. In addition to properties that show response headers, you can make a `Help/RateLimits` query to find rate limits on all or a filtered set of Twitter APIs. See [[Getting Rate Limits]] for more information.  You can find more information on how Twitter handles rate limiting in the Twitter API documentation: https://developer.twitter.com/en/docs/basics/rate-limiting.

### 4. Why are `ScreenName` and/or `UserID` properties `null `in the `User` entity response from Twitter?

The `ScreenName` and `UserID` properties are input only, allowing you to see the parameters you provided in your query. 

* For v2.1.x, use the `Identifier` property, which has the `ScreenName` and `UserID` returned from Twitter.

* For v3.0.x and later, use the `ScreenNameResponse` and `UserIDResponse` properties.

A bit of background: Anything used as an input parameter is also looked at in the query response, so if a user omits the parameter in a query but the twitter response contains a value, it was being filtered out of the results. To fix this, I adopted a convention where any return parameters also match input parameters would have a 'Response' suffix. e.g. `ScreenName` (input) and `ScreenNameResponse` (output). To find which values are input, the docs for each API call contain the input/filter parameters.

### 5. How do I page through `Status` and `Search` timelines?

Please see the blog post, [Working with Timelines with LINQ to Twitter](http://geekswithblogs.net/WinAZ/archive/2012/09/02/working-with-timelines-with-linq-to-twitter.aspx), that explains how to do this.

### 6. I'm not using `async/await` and my LINQ to Twitter code isn't working.

You must use `async/await` with LINQ to Twitter v3.0 or later. You might see deadlocks, race conditions, or undetected unhandled exceptions if you try to use LINQ to Twitter like previous versions. LINQ to Twitter v3.0 is asynchronous. That means you need `async` methods that use `await` on queries and commands. You also need to use exception handling (aka `try/catch`), especially for queries that throw exceptions for _not found_ conditions. The downloadable source code has a set of projects with the _Linq2TwitterDemos__ prefix for several technologies to demonstrate how to use `async` and `await` with LINQ to Twitter.

### 7. I'm receiving an error on assemblies named `System.Net.Http...` when trying to build.

LINQ to Twitter v3.0 has dependencies on `HttpClient`, which includes `System.Net.Http`, `System.Net.Http.Extensions`, and `System.Net.Http.Primitives`. You can add references to these assemblies via NuGet:

  1. Right-click on _References_ and select _Manage NuGet Packages_.
  2. Search for _HttpClient_.
  3. Install the _Microsoft HttpClient_ package.

Another reason for a related error, "System.IO.FileNotFoundException: Could not load file or assembly System.Net.Http.Primitives", occurs when a different version is in the GAC. You can solve that issue by adding a binding redirection to your `*.config` file, like this:
```xml
    <dependentAssembly>
        <assemblyIdentity name="System.Net.Http.Primitives" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.22.0" newVersion="4.2.22.0" />
    </dependentAssembly>
```
If you add LINQ to Twitter from NuGet, these dependencies should be added automatically. For more information on NuGet and versioning, David Ebbo has an excellent set of blog posts:

* [NuGet versioning Part 1: taking on DLL Hell](http://blog.davidebbo.com/2011/01/nuget-versioning-part-1-taking-on-dll.html)
* [NuGet versioning Part 2: the core algorithm](http://blog.davidebbo.com/2011/01/nuget-versioning-part-2-core-algorithm.html)
* [NuGet versioning Part 3: unification via binding redirects](http://blog.davidebbo.com/2011/01/nuget-versioning-part-3-unification-via.html)

### 8. How do I let a user authorize my app one time, without having to re-authorize every time.

The `CredentialStore` property of the `IAuthorizer` has the user's tokens, after authorization. So, after the initial authorization, read the tokens from the `CredentialStore` and save them in the database. Then check to see if you have tokens for the user the next time you authorize. If you have tokens, populate the authorizer. When the authorizer has all 4 tokens, LINQ to Twitter will allow you to perform queries without authorizing the user again. 

The C# console demos code contain the following:

```c#
            await auth.AuthorizeAsync();

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //
```

### 9. I'm writing an ASP.NET application and my credentials keep going away.

IIS recycles occasionally, which also means that any in-memory state is lost. This includes Session state if you're using _InProc_, which is the default. To fix this, reconfigure your application to use State Server or SQL Server. You can do a Bing search on how to configure ASP.NET Session State.

### 10. Can you write a sample for X?

You can download the source code and browse the documentation on this site to find samples on how to use each API. There are an untold number of scenarios that aren't covered, but I don't have the personal time to cover them all or keep up with requests. When you ask, I probably won't have the time to work up a sample and handle follow-up questions. By that time, you probably will have either figured out the problem or moved on to something else. While I would like to help, some things aren't practical, so I'll just have to wish you good luck.

### 11. I'm new to programming. Can you help me?

I've written LINQ to Twitter to help developers when writing applications that work with the Twitter API. Hopefully, the software makes their jobs easier. However, attempting to make a task easy doesn't imply it's appropriate for people with no experience. You must know: (1) how to code in C#, F#, or VB, (2) how LINQ works, (3) how to write async code, and (4) have some familiarity with the technology you're using (e.g. ASP.NET, WPF, Mobile, etc.). Without these things, you'll struggle. Of course, struggling is how we learn and make steps along the way to continually improve. This is what I do every day, continuously improving my own skills. While I would like to help, I don't have the time. However, I do know you will be able to figure it out if you stay with the journey.

### 12. How do I log into the Twitter API with Username and Password?

The Twitter API deprecated Username/Password authentication years ago. These days, you must use OAuth. You can visit [[Securing Your Applications]] for information on using OAuth with LINQ to Twitter. Additionally, there are several examples in the downloadable source code on how to use different types of authorizers with LINQ to Twitter. This is one of the more difficult tasks you will have when working with the Twitter API, or any other REST API that uses OAuth; so reading the documentation, evaluating alternatives, and reviewing the samples is all time well spent. It also helps if you get a sample application working first to get the hard part out of the way.

With all that said, Twitter API does have a feature named _XAuth_ that is based on _Username/Password_. However, it requires their permission to use. You will receive errors if you use `XAuthAuthorizor` and your account does not have Twitter API permission. You can visit https://developer.twitter.com and review their documentation on `XAuth` and investigate how to contact them if you want to ask for permission.

Note: Along the way, you're likely to encounter HTTP 401 Unauthorized errors and can get help for that in FAQ #1 above.

### 13. How do I do X, Y, or Z?

LINQ to Twitter has documentation available. You can find it by clicking the Wiki tab at the top of the page. There are also demos in the source code. It's usually helpful to get the demos running, including OAuth, to make sure you have your account, authorization, and credentials set up properly.


