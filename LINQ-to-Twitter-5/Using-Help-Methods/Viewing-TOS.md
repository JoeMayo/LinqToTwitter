#### Viewing TOS 

Displays the Twitter terms of service.

*Entity:* [Help](../LINQ-to-Twitter-Entities/Help-Entity.md)

*Type:* HelpType.Tos

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Tos
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null)
                Console.WriteLine(helpResult.Policies);
```

*Twitter API:* [help/tos](https://developer.twitter.com/en/docs/developer-utilities/terms-of-service/api-reference/get-help-tos)