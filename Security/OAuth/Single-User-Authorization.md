#### Implementing Single User Authorization

Single User Authorization is designed for scenarios where you'll only ever have one account accessing Twitter.  i.e. if your Web site does periodic Twitter updates, regardless of user or you have a server that monitors general information.  You can obtain all four Single User Credentials from your application page at http://dev.twitter.com.

To get started, you'll need to instantiate a _SingleUserAuthorizer_.  Credentials must be populated with ConsumerKey, ConsumerSecret, AccessToken, and AccessTokenSecret which identify both your application and the user to Twitter.  Heres how you can instantiate an authorizer:

```c#
    var auth = new SingleUserAuthorizer
    {
        CredentialStore = new SingleUserInMemoryCredentialStore
        {
            ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
            ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
            AccessToken = ConfigurationManager.AppSettings["accessToken"],
            AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
        }
    };
```
The example above instantiates a _SingleUserAuthorizer_ with an _SingleUserInMemoryCredentialStore_, which is an implementation of ICredentialStore. The credentials are being read from a config file, which is fine because they generally don't change for your application. The config file looks like this:
```xml
  <appSettings>
    <!-- Fill in your consumer key and secret here to make the OAuth sample work. -->
    <!-- Twitter sign-up: https://dev.twitter.com/ -->
    <add key="consumerKey" value="YourConsumerKey"/>
    <add key="consumerSecret" value="YourConsumerSecret"/>
    <add key="accessToken" value="AccessTokenFromTwitterAppPage"/>
    <add key="accessTokenSecret" value="AccessTokenSecretFromTwitterAppPage"/>
  </appSettings>
```

Unlike other authorizers, you don't need to authorize your application.  Just instantiate a TwitterContext with the authorizer and use it like this:

```c#
            var twitterCtx = new TwitterContext(auth);

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
You now have a _TwitterContext_ instance that has been properly authorized and can see how to use it to make queries.

#### Summary

The SingleUserAuthorizer allows you to fill in all of your credentials at one time, bypassing the user-centric authorization process.  The process is less involved than with other authorizers, such as PinAuthorizer or MvcAuthorizer, because the process only involves instantiating the authorizer and then assigning the authorizer to a TwitterContext instance.