#### Getting Rate Limits

Provides rate limits for each API endpoint.

*Entity:* [[Help|Help Entity]]
*Type:* HelpType.RateLimits

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Resources | Comma-separated list of endpoints to filter results on | string | no |

##### v3.0 Example:

```c#
            var helpResponse =
                await
                    (from help in twitterCtx.Help
                     where help.Type == HelpType.RateLimits
                     select help)
                    .SingleOrDefaultAsync();

            if (helpResponse != null && helpResponse.RateLimits != null)
                foreach (var category in helpResponse.RateLimits)
                {
                    Console.WriteLine("\nCategory: {0}", category.Key);

                    foreach (var limit in category.Value)
                    {
                        Console.WriteLine(
                            "\n  Resource: {0}\n    Remaining: {1}\n    Reset: {2}\n    Limit: {3}",
                            limit.Resource, limit.Remaining, limit.Reset, limit.Limit);
                    }
                }
```

##### v2.1 Example:

```c#
            var helpResult =
                (from help in twitterCtx.Help
                 where help.Type == HelpType.RateLimits //&&
                       //help.Resources == "search,users"
                 select help)
                .SingleOrDefault();

            foreach (var category in helpResult.RateLimits)
            {
                Console.WriteLine("\nCategory: {0}", category.Key);

                foreach (var limit in category.Value)
                {
                    Console.WriteLine(
                        "\n  Resource: {0}\n    Remaining: {1}\n    Reset: {2}\n    Limit: {3}",
                        limit.Resource, limit.Remaining, limit.Reset, limit.Limit);
                }
            }
```

*Twitter API:* [application/rate_limit_status](https://developer.twitter.com/en/docs/developer-utilities/rate-limit-status/api-reference/get-application-rate_limit_status)