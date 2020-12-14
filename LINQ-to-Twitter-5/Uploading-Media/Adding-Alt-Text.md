#### Adding Alt Text

Lets you tag media with alt tags for accessibility.

##### Signature:

```c#
public virtual async Task CreateMediaMetadataAsync(
    ulong mediaID, string altText, CancellationToken cancelToken = default(CancellationToken));
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| mediaID | ID of media. | ulong | yes |
| altText | Media description. | string | yes |
| cancelToken | Allows you to cancel async operation. | CancellationToken | no |

*Return Type:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

##### v4.x Example:

```c#
            if (mediaID == 0)
                throw new ArgumentNullException(nameof(mediaID), $"Invalid {nameof(mediaID)} parameter: {mediaID}");

            if (string.IsNullOrWhiteSpace(altText))
                throw new ArgumentNullException(nameof(altText), $"The {nameof(altText)} parameter is empty or null and must have a value.");

            string updateUrl = UploadUrl + "media/metadata/create.json";

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    updateUrl,
                    new MediaMetadata { MediaID = mediaID, AltText = new AltText { Text = altText } },
                    cancelToken)
                   .ConfigureAwait(false);
```

*Twitter API:* [media/metadata/create](https://developer.twitter.com/en/docs/media/upload-media/api-reference/post-media-metadata-create)