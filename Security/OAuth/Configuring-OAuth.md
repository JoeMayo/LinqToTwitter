#### Configuring OAuth

The starting point of using OAuth is to visit Twitter and register an application.  From there, you'll receive keys that will allow you to use OAuth with Twitter.  How you manage those keys will be up to you and can differ by application type, but this section will show you a simple example that will give you at least one idea to get started with.

##### Registering an Application

Twitter controls who can use it's service, which enables them to minimize the bad applications that can do harm to their service.  The controls in place are that you must register your application.  Here are a few tips for helping you get started with Application Registration:

1. Your application can be anything such as Joe's Test App, C# Station, or anything you want to name it.  Getting started, Twitter doesn't have any (initial) restrictions on the apps you create.
2. Visit [The Twitter Developer Site](http://dev.twitter.com) to register. You should already have a Twitter account so you can log in.  At this point, make sure it's a Twitter account you will either run the application on or test with; The reason being that many API calls default to the context of the logged in user.  If the account you use is your own personal account, you might easily end up spamming your followers with test content as you build your application.  Whichever way you decide has no impact on whether you can register an app, but this is an interesting situation to be aware of.
3. After you log in, visit Your Apps and click on Register a New App.
4. Most of the fields are self explanatory, but a couple are worth discussion because they can cause you many hours of discomfort trying to discover why your application isn't working:
5. Callback URL can be any URL you want.  LINQ to Twitter defaults to using the runtime Web page URL explicitly as the callback. You should always set this, especially if you're building a Web app.
6. If you want to be able to use all APIs, set Default access type to "Read, Write, and Direct Messages".  A Direct Message (DM) is a private message between users, which is supported by LINQ to Twitter.  If this is not turned on, Twitter will return security errors if you use the DM APIs.  If you missed setting this field, Twitter defaults to read only, which means that you can only do queries.  If you want to Tweet, you need to at least set "Read & Write".  Past problems associated with this materialize in the form of someone who can do queries, such as Public, Friends, and Search, but they can't do an Update.  The solution is to set this value to a value that provides more access.
7. After you fill in the fields, click Register Application, and you'll have immediate access to the Twitter API.

##### Managing Keys
After registering your application, you can obtain your OAuth keys. There are two keys you need for all OAuth scenarios, ConsumerKey and ConsumerSecret.  You'll find the keys by visiting the Twitter Developer site, selecting My apps, and clicking the title of your application.  You will put these keys in the appSettings section of app.config, web.config, or elsewhere depending on what type of application you're building.  You could even create your own custom key store, but the goal is to make them available to your application.  LINQ to Twitter will use these keys to authenticate via OAuth with Twitter.

If you're using SingleUserAuthorizer, visit your application page on Twitter to obtain your AccessToken and AccessTokenSecret keys. There's a button to generate these keys.  Then add the AccessToken and AccessTokenSecret keys to your configuration, in addition to ConsumerKey and ConsumerSecret keys.
##### Summary
Now you have a registered application and keys to help you authenticate with Twitter via OAuth using LINQ to Twitter.  Subsequent sections show you how to use LINQ to Twitter to perform this authentication.  The examples and discussion will assume that you've already registered your application, obtained your keys, and added your keys to a config file.
