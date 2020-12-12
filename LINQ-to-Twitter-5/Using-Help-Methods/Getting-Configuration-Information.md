#### Getting Configuration Information

Pulls general configuration information such as URL lengths, system names, and photo sizes.

*Entity:* [Help](../LINQ-to-Twitter-Entities/Help-Entity.md)

*Type:* HelpType.Configuration

##### Parameters/Filters:

None

##### v3.0 Example:

```c#
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Configuration
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null && 
                helpResult.Configuration != null && 
                helpResult.Configuration.NonUserNamePaths != null && 
                helpResult.Configuration.PhotoSizes != null)
            {
                Configuration cfg = helpResult.Configuration;

                Console.WriteLine("Short URL Length: " + cfg.ShortUrlLength);
                Console.WriteLine("Short URL HTTPS Length: " + cfg.ShortUrlLengthHttps);
                Console.WriteLine("Non-UserName Paths: ");
                foreach (var name in cfg.NonUserNamePaths)
                {
                    Console.WriteLine("\t" + name);
                }
                Console.WriteLine("Photo Size Limit: " + cfg.PhotoSizeLimit);
                Console.WriteLine("Max Media Per Upload: " + cfg.MaxMediaPerUpload);
                Console.WriteLine(
                    "Characters Reserved Per Media: " + cfg.CharactersReservedPerMedia);
                Console.WriteLine("Photo Sizes");
                foreach (var photo in cfg.PhotoSizes)
                {
                    Console.WriteLine("\t" + photo.Type);
                    Console.WriteLine("\t\t" + photo.Width);
                    Console.WriteLine("\t\t" + photo.Height);
                    Console.WriteLine("\t\t" + photo.Resize);
                } 
            }
```

##### v2.1 Example:

```c#
var helpResult =
    (from test in twitterCtx.Help
    where test.Type == HelpType.Configuration
    select test)
    .SingleOrDefault();

Configuration cfg = helpResult.Configuration;

Console.WriteLine("Short URL Length: " + cfg.ShortUrlLength);
Console.WriteLine("Short URL HTTPS Length: " + cfg.ShortUrlLengthHttps);
Console.WriteLine("Non-UserName Paths: ");

foreach (var name in cfg.NonUserNamePaths)
{
    Console.WriteLine("\t" + name);
}

Console.WriteLine("Photo Size Limit: " + cfg.PhotoSizeLimit);
Console.WriteLine("Max Media Per Upload: " + cfg.MaxMediaPerUpload);
Console.WriteLine("Characters Reserved Per Media: " + cfg.CharactersReservedPerMedia);
Console.WriteLine("Photo Sizes");

foreach (var photo in cfg.PhotoSizes)
{
    Console.WriteLine("\t" + photo.Type);
    Console.WriteLine("\t\t" + photo.Width);
    Console.WriteLine("\t\t" + photo.Height);
    Console.WriteLine("\t\t" + photo.Resize);
}
```

*Twitter API:* [help/configuration](https://developer.twitter.com/en/docs/developer-utilities/configuration/api-reference/get-help-configuration)