#### Viewing Privacy

Displays the Twitter privacy policy.

*Entity:* [Help](../LINQ-to-Twitter-Entities/Help-Entity.md)

*Type:* HelpType.Privacy

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Privacy
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null)
                Console.WriteLine(helpResult.Policies);
```

*Twitter API:* [help/privacy](https://developer.twitter.com/en/docs/developer-utilities/privacy-policy/api-reference/get-help-privacy)