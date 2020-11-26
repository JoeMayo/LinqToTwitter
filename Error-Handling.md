## Error Handling

In a perfect world, you would build a query with LINQ to Twitter and it would work flawlessly the first time. The reality is that you might encounter errors, especially when you first get started. The nature of these errors can vary and can be confusing and this section attempts to help you find and debug problems.

In particular, the errors that the Twitter API returns may be new to you. I'll cover a general approach to get started in understanding errors that can occur. Then I'll follow-up with some common scenarios and how you can approach them.

### Examining Exceptions

The most likely error you'll receive will be via an exception thrown from LINQ to Twitter. As with any other .NET library, an Exception means that the program was unable to perform it's intended purpose. Though potentially possible, It doesn't mean LINQ to Twitter or the Twitter API has an error - your program or configuration can be causing the error. The exception is the mechanism by which you are able to learn more information about what the problem could be.

In LINQ to Twitter, you'll receive a TwitterQueryException. The Message and ErrorCode properties contain the error message and error code, respectively, from the Twitter API. You should read these to see if they indicate what the problem could be. You can visit the Twitter documentation for [Error Codes & Responses](https://dev.twitter.com/overview/api/response-codes) to see what the possible errors are. A few examples are 88/Rate Limit Exceeded, meaning that you've sent more requests during a 15 minute window than what you're allowed; 187/Status is a duplicate, meaning that you tweeted the exact same text twice; or 32/Could not authenticate you, meaning that Twitter wasn't able to verify who you are.

Another property of TwitterQueryException that you should look at is InnerException, which is the original exception that caused the problem. Sometimes an exception is due to a network problem or something else that has nothing to do with Twitter. The inner exception is important for helping figure out the problem and you should examine all the properties of that exception too.

### Example Scenarios

Here are a couple potential scenarios and actions you can take to fix the problem.

#### HTTP 401 Unauthorized

The 401 error is one of the most frequent problems that people have. You can tell if the problem is a 401 by looking at the StatusCode on the exception. It means that you weren't able to authorize the application. There are many different reasons this can happen and figuring out the problem can be complex because there could actually be multiple things at play. Over the years, I've collected reasons why a 401 can occur and compiled the results into the [[LINQ to Twitter FAQ]]. Please visit the FAQ and review each step to resolve the problem.

#### Rate Limits

Twitter rate limits the API, meaning that you have _n_ queries for each 15 minutes. If you exceed those rates, you'll receive an error from Twitter. The Twitter documentation, [API Rate Limits](https://dev.twitter.com/rest/public/rate-limiting), explains how rate limiting works. You can use the LINQ to Twitter documentation, [[Getting Rate Limits]], to find out what your limits are for each API.