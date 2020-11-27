#### Implementing OAuth for Desktop Applications

With desktop applications, such as Console, Windows Forms, or Windows Presentation Foundation (WPF), you don't have the full Web-based OAuth flow for authenticating an application. Specifically, after the user authorizes the application, there isn't a Web page to redirect back to for getting the final access token. To solve this problem, Twitter offers PIN authorization, which bridges the gap between user authorization and application notification that it can get the final access token.  This page explains how PIN authorization works in LINQ to Twitter.  Before jumping into code, here's a high-level description of the workflow for PIN authorization:

#### High-Level Description of PIN Authorization

You'll see the details of how PIN authorization is implemented with LINQ to Twitter soon, but it might be useful to review how the whole process works.  It might help you know where you're at in the process when looking at later details.  Here's what happens during PIN authorization.

1. Some action in your code (login or a specific need to use a Twitter feature that requires authorization) initiates the authorization process.

2. Your code tells LINQ to Twitter to initiate the authorization process.

3. Your code waits for a PIN to be entered.

4. LINQ to Twitter redirects the user to the Twitter authorization page.

5. The user authorizes your application.

6. Twitter redirects to a page with a PIN Number.

7. The user manually gives that PIN number to your application, which is waiting for it to be entered.

8. When authorization is complete, OAuth and Access tokens are available for you to store for this user.

Of all the previous steps, #1 and #8 are less defined.  That's because those are the points that depend on how you design your application.  After authorization, LINQ to Twitter can be used for any queries or side-effects.

Now that you have an idea of how the process works, let's look at an examples of how you can code this with LINQ to Twitter.

##### [[Implementing PIN Authorization for Console Applications]]

##### Summary

The high-level view explained how the PIN authorization process works, which  is different from Web authorization.  The next section on implementation showed one way to write code for PIN authorization.  The PinAuthorizer instantiation is where most of the setup occurs. In particular, remember the extensibility points with IOAuthCredentails, GoToTwitterAuthorization, and GetPin so you can customize PIN authorization for your particular application. The rest of the process simply involved calling Authorize and passing the authorizer to a new TwitterContext instance.