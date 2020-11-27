#### Implementing PIN Authorization for Console Applications

To get started, you'll need to instantiate a PinAuthorizer, which is a LINQ to Twitter type that helps you perform PIN authorization with Twitter.  PinAuthorizer requires a ConsumerToken and ConsumerSecret Key pair to start the process.  Additionally, you tell PinAuthorizer how to redirect to the Twitter authorization page and how you'll accept the PIN after the user authorizes.  The following code shows how to do this:

```c#
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };
```
The code above uses object initialization syntax to set up CredentialStore, GoToTwitterAuthorization, and GetPin.  This example uses InMemoryCredentialStore, which is a LINQ to Twitter type that implements ICredentialStore. The example uses configuration settings to load the ConsumerKey and ConsumerSecret, which identify your application to Twitter.

LINQ to Twitter needs to know how you want to bring the user to the Twitter authorization Web page. The example above, setting GoToTwitterAuthorization with an Action<string> lambda to implement Process.Start is one way.  The pageLink parameter is the HTTP address of Twitter's authorization page, so the effect will be to launch the user's browser and navigate to the Twitter authorization page.

After the user authorizes your application, your application must have a way to get the PIN from the user.  The example above shows how to do this through the console.

Essentially, LINQ to Twitter will execute the lambda assigned to GoToTwitterAuthorization and then execute the lambda assigned to GetPin.  These are extensibility points that allow you to provide your own implementation.

Setting up PinAuthorizer is the most work you'll do for OAuth authorization. The rest of the code required is to kick off the authorization process and then instantiate TwitterContext.

Here's how to kick off authorization.

```c#
            await auth.AuthorizeAsync();
```
As mentioned earlier, LINQ to Twitter will execute the GoToTwitterAuthorization authorizer lambda so the user can authorize your application.  Then it will execute the GetPin lambda to collect the PIN number.  Everything else happens behind the scenes.

After the call to AuthorizeAsync above, you can read the OAuthToken and AccessToken from auth.CredentialStore and save them in a database, associated with the current user.  On subsequent queries to LINQ to Twitter, you can read those values from the database and populate the Credentials property of PinAuthorizer ahead of time.  This will prevent LINQ to Twitter from making the user perform the authorization process again and you'll be able to start making queries on behalf of that user right away.

After AuthorizeAsync completes, instantiate a TwitterContext, like this:

```c#
            using (var twitterCtx = new TwitterContext(auth))
            {
                //Log
                twitterCtx.Log = Console.Out;

                // LINQ to Twitter query goes here
            }
```
The code above passes the PinAuthorizer instance, _auth_, to the TwitterContext constructor.  After this, you can query Twitter as normal.

For debugging, you can set the Log property. The code above uses Console.Out, but you're free to implement and/or assign your TextWriter of choice.