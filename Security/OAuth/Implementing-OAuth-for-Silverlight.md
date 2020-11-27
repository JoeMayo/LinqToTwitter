#### Implementing OAuth for Silverlight Applications

Silverlight authorization is typically more complex because of security issues and the type of Silverlight application being built.  For example, you can't communicate directly with Twitter from a Silverlight page and will need a server proxy.  Also, you have a choice of Web or Out-Of-Browser (OOB) application, each with a separate OAuth authorizer. This page will explain how you can implement OAuth in Silverlight, supporting different scenarios.  Before jumping into code, you'll see a high-level view of how the whole process works.

##### High-Level Description of Silverlight Authorization

You'll see the details of how Silverlight authorization is implemented with LINQ to Twitter soon, but it might be useful to review how the whole process works.  It might help you know where you're at in the process when looking at later details.  The following example shows how to perform Silverlight authorization for Web applications and will be followed by an explanation of the differences to support OOB apps:

1. Some action in your code (login or a specific need to use a Twitter feature that requires authorization) initiates the authorization process.

2. Your code tells LINQ to Twitter to begin the authorization process.

3. LINQ to Twitter redirects the user to the Twitter authorization page.

4. The user authorizes your application.

5. Twitter redirects to a callback page in your application.

6. Your code tells LINQ to Twitter to complete the authorization process.

7. When authorization is complete, OAuth and Access tokens are available for you to store for this user.

Of all the previous steps, #1 and #7 are less defined.  That's because those are the points that depend on how you design your application.  After authorization, LINQ to Twitter can be used for any supported queries or side-effects.

The differences between the Web application process above and OOB occurs at step #5.  Instead of redirection, Twitter will redirect the user to a page with a PIN number.  Your OOB application must be designed to allow the user to enter the PIN number before continuing - details which you'll see soon.

Now that you have an idea of how the process works, let's look at an example of how you can code this with LINQ to Twitter.

##### Required Configuration and Setup

Silverlight applications don't have access to communicate directly with Twitter.  The way to work around this in LINQ to Twitter is via a proxy.  The proxy is an ASP.NET HTTP Handler.  LINQ to Twitter forwards all of it's communication to Twitter through this proxy.  To use the proxy, copy LinqToTwitterProxy.ashx and LinqToTwitterProxy.ashx.cs to the root folder of your Web application that hosts your Silverlight application.  You can find the proxy files in the downloadable LINQ to Twitter source code, in the Silverlight web demo project.

Important for OOB: If you're going to support OOB, you'll need to configure properties for the Silverlight project.  Open Properties, Silverlight tab, and make sure Enable running application out of the browser is checked.  Then click the Out of browser settings button and ensure the Require elevated trust when running out of the browser is checked.

##### Implementing Silverlight Authorization

The example for this page is a Silverlight navigation application.  Further, the example shows how to support both Web and OOB applications at the same time.  Therefore, you can use what you learn here to support OOB, Web, or both.  Navigation applications have an OnNavigatedTo override that's called whenever the user navigages to a page. This example will use this method to initiate the Silverlight authorization process.  Here's the OnNavigatedTo implementation:

```c#
    public partial class FriendsStatusQuery : Page
    {
        private TwitterContext m_twitterCtx = null;
        private PinAuthorizer m_pinAuth = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser &&
                Application.Current.HasElevatedPermissions)
            {
                DoPinAuth(); 
            }
            else
            {
                DoWebAuth();
            }
        }
```
The code above demonstrates how to determine whether this application is running OOB or via Web.  It's important the make the settings in the previous section if the application supports running OOB. We'll discuss the Web scenario first.

When performing authorization for a Silverlight Web application, the code calls the DoWebAuth method, above.  DoWebAuth contains all of the logic for the OAuth process, including Authorizer initialization, beginning the authorization process, and completing the authorization process.  The first step is to instantiate an authorizer, which requires application credentials and instructions on how the application should show the Twitter authorization page to the user.  The following code shows how to instantiate the authorizer.

```c#
        private void DoWebAuth()
        {
            WebBrowser.Visibility = Visibility.Collapsed;
            PinPanel.Visibility = Visibility.Collapsed;

            var auth = new SilverlightAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                PerformRedirect = authUrl => 
                    Dispatcher.BeginInvoke(() => HtmlPage.Window.Navigate(new Uri(authUrl)))
            };
```
In the code above, ConsumerKey and ConsumerSecret are set to blank strings, but you would load your own application credentials here. When the authorization process starts, LINQ to Twitter will show the authorization page to the user by executing the Action<string> lambda assigned to PerformRedirect.  You see, LINQ to Twitter doesn't know exactly how you want to implement this part of your application, so you have the freedom to define it as you like.  In the example above, the authUrl parameter is the HTTP address of the Twitter authorization page. The lambda behavior is to redirect the browser to that page.  As is common with Silverlight applications, Dispatcher.BeginInvoke marshalls the operation onto the UI thread.

Now you have a SilverlightAuthorizer instance and are ready to begin the authorization process.  The next part of the code might seem a little backwards because it calls CompleteAuthorization before BeginAuthorization, but there's a reasonable explanation for this.  The true order of operations is that BeginAuthorization runs first, the application redirects to the Twitter authorization page, the user authorizes the application, Twitter redirects back to this page (back to OnNavigatedTo, which calls DoWebAuth), and CompleteAuthorization executes.  Here's the code:

```c#
            Uri url = HtmlPage.Document.DocumentUri;

            auth.CompleteAuthorize(url, resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    // you can save these in the db and add logic
                    // to look for credentials from db before
                    // doing Oauth dance.
                    var oauthToken = auth.Credentials.OAuthToken;
                    var accessToken = auth.Credentials.AccessToken;

                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            FriendsPanel.Visibility = Visibility.Visible;
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                resp.Error.ToString(),
                                resp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));

            if (!auth.IsAuthorized && !auth.IsAuthorizing)
            {
                auth.BeginAuthorize(url, resp =>
                    Dispatcher.BeginInvoke(() =>
                    {
                        switch (resp.Status)
                        {
                            case TwitterErrorStatus.Success:
                                break;
                            case TwitterErrorStatus.TwitterApiError:
                            case TwitterErrorStatus.RequestProcessingException:
                                MessageBox.Show(
                                    resp.Error.ToString(),
                                    resp.Message,
                                    MessageBoxButton.OK);
                                break;
                        }
                    }));
            }
```
Inside of CompleteAuthorization, there's a check that ensures the logic executes only if BeginAuthorization has already been called. This logic also indicates that authorization is complete, which prevents BeginAuthorization from being called again.  Otherwise, it just returns.  BeginAuthorize only runs if the application isn't authorized.  BeginAuthorization takes a callback parameter, which is passed to Twitter to indicate where the completion process is.  Authorization for this sample begins and completes on the same page, as you can see at the top of the listing where url is set to the HTTP address of the current page.

Notice the async callbacks on both BeginAuthorization and CompleteAuthorization.  Each has a switch statement for processing the results of the operation, returned via the Status property of the TwitterAsyncResponse, resp.  Besides error processing, the CompleteAuthorization callback shows how to get values for OAuthToken and AccessToken when authorization is successful.  You can store these tokens in the database, associated with the user, and retrieve them on subsequent calls.  If you load all four credentials into SilverlightAuthorizer at one time, LINQ to Twitter allows you to bypass the authorization process and perform queries immediately.  Normally, you only have to go through the authorization process one time per user.

Once the application is authorized, pass the authorizer to a new TwitterContext instance, like this:

```c#
            m_twitterCtx = new TwitterContext(auth);
        }
```
The m_twitterCtx is a private field, allowing access via other page methods for queries.  As shown by the curly brace, this is the end of the DoWebAuth method.  Next, you'll learn how to support authorization of OOB apps via the DoPinAuth method.

When running OOB, you don't have the full authorization workflow because the application is running on the desktop.  After the user authorizes your application with Twitter, there isn't a way for Twitter to redirect to your desktop.  Therefore, you can use PIN authorization.  The first step in PIN authorization is to instantiate an authorizer, which requires application credentials and instructions on how the application should show the Twitter authorization page to the user.  The following code shows how to instantiate the authorizer.

```c#
        private void DoPinAuth()
        {
            m_pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "",
                    ConsumerSecret = ""
                },
                UseCompression = true,
                GoToTwitterAuthorization = pageLink =>
                    Dispatcher.BeginInvoke(() => WebBrowser.Navigate(new Uri(pageLink)))
            };
```
In the code above, ConsumerKey and ConsumerSecret are set to blank strings, but you would load your own application credentials here. When the authorization process starts, LINQ to Twitter will show the authorization page to the user by executing the Action<string> lambda assigned to GoToTwitterAuthorization.  You see, LINQ to Twitter doesn't know exactly how you want to implement this part of your application, so you have the freedom to define it as you like.  In the example above, the authUrl parameter is the HTTP address of the Twitter authorization page. The lambda behavior is to redirect the browser to that page.  As is common with Silverlight applications, Dispatcher.BeginInvoke marshalls the operation onto the UI thread.

The next step is to begin the authorization process by calling BeginAuthorization, shown below:

```c#
            m_pinAuth.BeginAuthorize(resp =>
                Dispatcher.BeginInvoke(() =>
                {
                    switch (resp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                resp.Error.ToString(),
                                resp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));

            m_twitterCtx = new TwitterContext(m_pinAuth, "https://api.twitter.com/1/", "https://search.twitter.com/");
        }
```
In PIN authorization, you don't pass a URL to BeginAuthorization because there isn't a callback.  BeginAuthorization executes the lambda assigned to GoToTwitter, the user authorizes your application, and Twitter redirects the user to a page that contains a PIN. The code assigns the authorizer to a new instance of TwitterContext, but you can't use it until after authorization and that occurs in a separate method.

Because you don't have a callback, the OOB application must wait on the user to return to the application and type in the PIN that Twitter provided.  Also, you can't complete the authorization process until the PIN is available, to trying to handle begin and complete in the same method doesn't work for OOB PIN authorization applications.  The way this example handles this situation is to show a TextBox for the PIN and a button that the user clicks after they enter the PIN into the TextBox.  Here's the event handler for that button click that completes the authorization process:

```c#
        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            string pin = PinTextBox.Text;

            m_pinAuth.CompleteAuthorize(
                PinTextBox.Text,
                completeResp => Dispatcher.BeginInvoke(() =>
                {
                    // you can save these in the db and add logic
                    // to look for credentials from db before
                    // doing Oauth dance.
                    var oauthToken = auth.Credentials.OAuthToken;
                    var accessToken = auth.Credentials.AccessToken;

                    switch (completeResp.Status)
                    {
                        case TwitterErrorStatus.Success:
                            FriendsPanel.Visibility = Visibility.Visible;
                            break;
                        case TwitterErrorStatus.TwitterApiError:
                        case TwitterErrorStatus.RequestProcessingException:
                            MessageBox.Show(
                                completeResp.Error.ToString(),
                                completeResp.Message,
                                MessageBoxButton.OK);
                            break;
                    }
                }));
        }
```
The code above reads the PIN from the TextBox.  CompleteAuthorize takes this PIN and requests an access token from Twitter.  

Both BeginAuthorize and CompleteAuthorize have a switch statement for processing the results of the operation, returned via the Status property of the TwitterAsyncResponse, resp.  Besides error processing, the CompleteAuthorization callback shows how to get values for OAuthToken and AccessToken when authorization is successful.  You can store these tokens in the database, associated with the user, and retrieve them on subsequent calls.  If you load all four credentials into SilverlightAuthorizer at one time, LINQ to Twitter allows you to bypass the authorization process and perform queries immediately.  Normally, you only have to go through the authorization process one time per user.

You can now pass this authorizer to an instance of TwitterContext and perform queries.

##### Summary

The high-level view explained how the Silverlight authorization process works.  The next section on implementation showed how to perform authorization for both OOB and Web Silverlight applications.  The SilverlightAuthorizer was instantiated with Credentials and PerformRedirect and the PinAuthorizer was instantiated with Credentials and GoToTwitterAuthorization. In particular, remember the extensibility points with IOAuthCredentails and PerformRedirect/GoToTwitterAuthorization so you can customize Web authorization for your particular application. Pay attention to how the BeginAuthorization and CompleteAuthorization in Web authorization occur in different places because of browser redirection and using the same method from the call to OnNavigateTo.  For OOB applications, notice that BeginAuthorization occurs in DoPinAuth, but CompleteAuthorization executes in an event handler, allowing authorization to complete only after the user has provided the PIN.