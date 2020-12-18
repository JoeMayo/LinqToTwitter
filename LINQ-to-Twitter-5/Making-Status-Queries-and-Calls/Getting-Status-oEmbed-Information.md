#### Getting Status oEmbed Information

Gets embeddable status information.

*Entity:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

*Type:* StatusType.Oembed

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| ID | Tweet ID | ulong | yes |
| OEmbedUrl | Tweet URL | string | yes |
| OEmbedAlign | Embedded tweet alignment | EmbeddedStatusAlignment | no |
| OEmbedHideMedia | Hide attached media | bool | no |
| OEmbedHideThread | Hide thread | bool | no |
| OEmbedLanguage | Language code for the embedded HTML | string | no |
| OEmbedMaxWidth | Width of embedded tweet | int | no |
| OEmbedOmitScript | Hide < script > in the returned HTML | bool | no |
| OEmbedRelated | Related parameter value | string | no |

##### v3.0 Example:

```c#
            ulong tweetID = 305050067973312514;

            var embeddedStatus =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Oembed &&
                       tweet.ID == tweetID
                 select tweet.EmbeddedStatus)
                .SingleOrDefaultAsync();

            if (embeddedStatus != null)
                Console.WriteLine(
                    "Embedded Status Html: \n\n" + embeddedStatus.Html);
```

##### v2.1 Example:

```c#
            var embeddedStatus =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Oembed &&
                       tweet.ID == "305050067973312514"
                 select tweet.EmbeddedStatus)
                .SingleOrDefault();

            Console.WriteLine("Embedded Status Html: " + embeddedStatus.Html);
```

*Twitter API:* [statuses/oembed](https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/get-statuses-oembed)