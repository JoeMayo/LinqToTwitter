#### Retrieving Languages

Provides a list of languages supported by Twitter.

*Entity:* [[Help|Help Entity]]
*Type:* HelpType.Languages

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Languages
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null && helpResult.Languages != null)
                helpResult.Languages.ForEach(lang => 
                    Console.WriteLine("{0}({1}): {2}", lang.Name, lang.Code, lang.Status));
```

##### v2.1 Example:

```c#
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Languages
                 select test)
                .SingleOrDefault();

            foreach (var lang in helpResult.Languages)
            {
                Console.WriteLine("{0}({1}): {2}", lang.Name, lang.Code, lang.Status);
            }
```

*Twitter API:* [help/languages](https://developer.twitter.com/en/docs/developer-utilities/supported-languages/api-reference/get-help-languages)