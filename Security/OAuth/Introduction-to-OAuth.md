#### Introduction to OAuth

_OAuth_ is a security protocol that allows applications to act on your behalf without requiring you to give out your password to every application. This definition is a generalization and simplification of what OAuth truly is, so this discussion will expand on what OAuth is and it's mechanics. I'll start with an example and explain why OAuth is important and what problem it solves. The real-life example I'll use is from the [C# Station Web site](http://www.csharp-station.com/), which uses OAuth to enable visitors to tweet their success upon completing the C# Tutorial. With this example in mind, you'll learn the communications flow and security information exchanged between C# Station and Twitter, allowing C# Station to post a tweet on the user's behalf. Then, I'll follow-up with a summary, reinforcement of the importance of OAuth, and references on where you can go next to continue your journey in making OAuth work for you.

##### Why is OAuth Important?

There is an increasing need for Web applications to communicate with each other.  In the Twitter space, there are literally hundreds of 3rd party application that access Twitter on your behalf to provide functionality. Examples include posting photos, scheduling tweets, and full-featured clients.  Many of these applications fill holes in Twitter's offerings and deliver value, contributing to their popularity.  Without OAuth, each of these applications would need to collect your username and password to interact with Twitter on your behalf and therin lies the problem.

Sharing secrets (credentials) with a 3rd party application requires putting trust in that application to act responsibly. This is a risky proposition because eventually some unscrupulous application will misuse your secrets for their own gain or engage in malicious behavior. Therefore, you need to minimize who you share your secrets with. Adding to the unfortunate circumstances of finding yourself in a position where an application has misused secrets, the first line of defense is to change passwords. The problem with changing passwords is that the same password has been given to multiple other applications. Therefore, after changing your Twitter password, you also need to change passwords for every other application.  Giving out secrets to every application is fraught with risk and complication.

OAuth is a way to enable the scenario of working with 3rd party applications, without giving out your secrets.  Essentially, a 3rd party application that wants to access your Twitter account, using OAuth, will perform a redirection to an authorization page on Twitter, you will then tell Twitter to give them permission, and the application will be able to perform actions on your behalf.  This whole sequence of events occurs without needing to share a password with the 3rd party site.  Instead, Twitter has shared a token to your account that the 3rd party application uses.  If later, you find that you can't trust the 3rd party application, go to Twitter and cancel their access and Twitter will no longer allow that specific application to access your account. Because access is controlled through Twitter, you don't have to do anything special for other applications because they still have access to your account.  No one has your Twitter password and you don't have to encounter the pain of visiting every site.

There was a time when you could use basic authentication (username/password). However, that capability was deprecated and no longer available. While OAuth is more challenging for the developer, it's a superior solution in the best interest of end users.

##### Examining a Real-World Use of OAuth

The example we'll use is from the [C# Station Web Site](http://www.csharp-station.com/).  Whenever a user completes the C# Tutorial, the last page contains a link for tweeting their success. This is similar to the functionality of many 3rd party applications today that allow you to tweet some type of status related to what you're doing with that application. The following steps show you how the C# Station Tutorial success tweet works:

1. On the page of the last C# Tutorial lesson, the user clicks on the "Tweet Your Success!" link. They'll see the following page, which explains that clicking the _Announce_ button will use OAuth to let them give C# Station permission to tweet on their behalf:

[image:The Tweet Your Success Page where the user initiates an OAuth session|OAuthCSharpStationStart.png]

2. Assuming the user decides they want to let the world know of their accomplishment, they will click the _Announce_ button, which will lead them to the following Twitter page to give C# Station permission to tweet their message:

[image:The Twitter OAuth Authorization page where the user gives C# Station permission to tweet for them|OAuthCSharpStationTwitterAuth.png]

3. As shown above, the user fills in their Twitter username and password.  Notice that they are only sharing their password with Twitter, not the 3rd party application.  If the user is already logged into Twitter, they will only see the _Deny_ and _Allow_ buttons. As a side note: a user should inspect the URL of the site to which they were sent to avoid a phishing attach, just in case the site isn't quite legitimate. Alternatively, a user can visit Twitter themselves to log in ahead of time, avoiding the need to provide credentials during the OAuth session.

If the user clicks _Deny_, the site will not be able to access the user's Twitter account.  Clicking Allow causes Twitter to redirect the user's browser back to the 3rd party application's site to continue whatever subsequent actions might be required. In the case of the current example, C# Station will post a success tweet to the user's account and let the user know, as shown below:

[image:Notifying the user that the their success has been tweeted|OAuthCSharpStationTweeted.png]

4. When the user returns to their Twitter account, they can see that their success has indeed been posted to their account, as shown in the following image:

[image:The Tweet appearing on user's Twitter page|OAuthCSharpStationResults.png]

Now you've seen an OAuth session in action and how the user didn't need to share their secrets with the 3rd party application. The next section shows this workflow with a sequence diagram, keeping the view at a high level.

##### A Birds-Eye View of OAuth Workflow

The workflow in the previous section demonstrated the user experience with OAuth. However, you'll be writing code to produce this experience yourself and need a view that is a step closer to code.  The following figure is a sequence diagram that shows the flow of communication between actors: the user, 3rd party application, and Twitter. This sequence diagram is a little more detailed because it shows what information is passing between the actors in the system. The information is either OAuth tokens or username/password credentials.  I'll explain more after you've taken a look at the following sequence diagram:

[image:The OAuth Process|OAuthSequence.png]

The sequence diagram above corresponds roughly to the description in the previous section, except you can now see the entire process altogether. As in clicking on the Announce button, you must have some action in your application that initiates the OAuth sequence. The application recognizes that command and redirects the user to the Twitter authorization page. After the user authorizes the application, Twitter redirects the user back to the application.  When writing the application, you will need to recognize the fact that Twitter has redirected back to you.  At that time, the application code will go to Twitter and perform whatever action it needs to and Twitter will permit the application to do so. Upon successfully completing the operation, the application will let the user know that it is done.

##### Summary

What you've seen in the previous sections is the application of OAuth from a user's perspective. Stepping through the process of how C# Station managed the OAuth process should give you a better idea of what the user experience is.  The sequence diagram was more-or-less a description of the previous steps and served as yet another view of how the process works from a high level.

At this point in time, you are probably curious about more of the details of how OAuth works and what does it take to write application code to make this sequence happen.  For example, how does the application know what page to go to, how does the application know that the user authorized it, and how does Twitter tell the difference between an authorized and unauthorized application request? These are all important questions, which you'll learn about in the next section.

##### References

[The Beginner's Guide to OAuth](http://hueniverse.com/oauth/)
[The Official OAuth Site](http://oauth.net/)
[The Twitter OAuth Page](http://dev.twitter.com/pages/auth)