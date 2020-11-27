#### Implementing OAuth for MVC Applications

MVC authorization includes the entire OAuth authorization flow.  This page will explain how you can implement OAuth in ASP.NET Model-View-Controller (MVC) using LINQ to Twitter's `MvcAuthorizer`.  Before jumping into code, you'll see a high-level view of how the whole process works.

##### High-Level Description of MVC Authorization

You'll see the details of how MVC authorization is implemented with LINQ to Twitter soon, but it might be useful to review how the whole process works.  It might help you know where you're at in the process when looking at later details.  Here's what happens during MVC authorization.

1. Some action in your code (login or a specific need to use a Twitter feature that requires authorization) initiates the authorization process.

2. Your code tells LINQ to Twitter to begin the authorization process.

3. LINQ to Twitter redirects the user to the Twitter authorization page.

4. The user authorizes your application.

5. Twitter redirects to a callback page in your application.

6. Your code tells LINQ to Twitter to complete the authorization process.

7. When authorization is complete, OAuth and Access tokens are available for you to store for this user.

Of all the previous steps, #1 and #7 are less defined.  That's because those are the points that depend on how you design your application.  After authorization, LINQ to Twitter can be used for any supported queries or side-effects.

Now that you have an idea of how the process works, let's look at an example of how you can code this with LINQ to Twitter.

##### Implementing MVC Authorization

This example is for a design that redirects the user to a unique controller, dedicated to the OAuth process, `OAuthController`. To get started, you'll need to instantiate an `MvcAuthorizer `with `CredentialStore`.  `CredentialStore `must be populated with `ConsumerKey `and `ConsumerSecret`, which identify your application to Twitter.  Here's how you can instantiate `MvcAuthorizer`:

```c#
    public class OAuthController : AsyncController
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BeginAsync()
        {
            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                }
            };
```
The example above instantiates a `SessionStateCredentialStore`, which is an implementation of `ICredentialStore`. The actual credentials are being read from `web.config`, which is fine because they generally don't change for your application. 

Tip: Since `SessionStateCredentialStore` stores credentials in Session state, you'll want to ensure you've set the mode to State Server or SQL Server, but not InProc to avoid intermittent loss of credentials.

After you have an `MvcAuthorizer `instance, begin the OAuth process by calling `BeginAuthorizationAsync`, like this:
```c#

            string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }
```
This code is under the `MvcAuthorizer` instantiation, inside of `BeginAsync`. Notice that I've taken the current URL, which points to the current controller and action method, and switched the text "Begin" for "Complete" for the `twitterCallbackUrl`. This lets LINQ to Twitter tell the Twitter API where to redirect the user after they have authorized your application. You call `BeginAuthorizationAsync` with this callback value and LINQ to Twitter will redirect the user to Twitter's authorization page, including the callback.

`BeginAuthorizationAsync` has a second parameter, of type `Dictionary<string, string>`, that allows you to send custom query string parameters. Twitter returns those values after a user authorizes your app and you can read them, via `Request.QueryString` in the `CompleteAsync` method. Here's a revised example:

```cs
            var parameters = new Dictionary<string, string> { { "my_custom_param", "val" } };
            string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl), parameters);
```

Setting the callback, as done in the previous section, causes Twitter to redirect the user to the `CompleteAsync` action, instead of what the original URL was for the `BeginAsync` action. Once the redirect occurs, the code in `CompleteAsync` executes and lets you finish the OAuth process, like this:
```c#
        public async Task<ActionResult> CompleteAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore()
            };

            await auth.CompleteAuthorizeAsync(Request.Url);

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, 
            //   isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent 
            //   queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let 
            //   you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            return RedirectToAction("Index", "Home");
        }
```
Here, you instantiate a new `MvcAuthorizer`, with a new instance of `SessionStateCredentialStore`. Since credentials are held in Session state, `SessionStateCredentialStore` automatically reads them. If all 4 credentials are available, LINQ to Twitter lets you perform queries without making the user go through the entire OAuth process again. 

However, at this point in time, you have `ConsumerKey`, `ConsumerSecret`, and OAuthToken, but you still don't have `AccessToken`. Call `CompleteAuthorizationAsync` to get the `AccessToken` and you'll have all 4 tokens available.

As noted in the comments, after `CompleteAuthorizeAsync` is a good time to collect the data from Twitter and store it for this user. Then on subsequent queries for this user, you can load all 4 credentials to avoid making the user go through the entire OAuth process.

This code redirects to another controller/action and you can write any code you want to redirect to the destination of your choice. After you're authorized, you can use LINQ to Twitter like normal. Here's an example that queries the user's Home timeline:

```c#
        public async Task<ActionResult> HomeTimelineAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore()
            };

            var ctx = new TwitterContext(auth);

            var tweets =
                await
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.Home
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.Text
                 })
                .ToListAsync();

            return View(tweets);
        }
```
Here you can see the instantiation of `MvcAuthorizer` with a new instance of `SessionStateCredentials`. If you've already authorized, `SessionStateCredentials` will pick up credentials from Session state. If the user has just logged onto your application, check to see if you have stored credentials for them (previously, in your database) or send them to the OAuth page for the first time. If you already have their credentials, then load them into the `ConsumerKey`, `ConsumerSecret`, `AccessToken`, and `AccessTokenSecret` properties of `SessionStateCredentials`.

Then instantiate a `TwitterContext` with the `MvcAuthorizer` instance and use LINQ to Twitter like normal. 

#### Summary

The high-level view explained how the MVC authorization process works.  The next section on implementation showed one way to write code for Web authorization.  It used a separate controller to simplify the OAuth process, redirecting back to another page to perform queries. Remember to store credentials in your database after `CompleteAuthorizationAsync` so you can load those credentials on subsequent sessions to create a better user experience.