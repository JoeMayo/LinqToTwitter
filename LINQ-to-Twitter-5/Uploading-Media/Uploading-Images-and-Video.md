#### Uploading Images and Video

Allows you to upload multiple images or a video.

##### Signature:

```c#
public virtual async Task<Media> UploadMediaAsync(
    byte[] media, string mediaType, string mediaCategory, 
    CancellationToken cancelToken = default(CancellationToken));

public virtual async Task<Media> UploadMediaAsync(
    byte[] media, string mediaType, IEnumerable<ulong> additionalOwners, 
    string mediaCategory, CancellationToken cancelToken = default(CancellationToken));
```

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| additionalOwners | User IDs of accounts that can used the returned media IDs. | IEnumerable<ulong> | no |
| media | Media to upload. | byte[] | yes |
| mediaType | Type of media. e.g. image/jpg, image/png, or video/mp4. | string | yes |
| mediaCategory | Media category - possible values are tweet_image, tweet_gif, tweet_video, and amplify_video. See this post on the Twitter forums: https://twittercommunity.com/t/media-category-values/64781/6 | string | sometimes |
| cancelToken | Allows you to cancel async operation | CancellationToken | no |

*Return Type:* [Status](../LINQ-to-Twitter-Entities/Status-Entity.md)

Note: The response you receive differs between images and videos. If uploading videos, the code below works well. However, if uploading images, you shouldn't iterate, waiting for processed status because that happens immediately. See this issue for more info:

* [Cannot upload image, Sorry, that page does not exist](https://github.com/JoeMayo/LinqToTwitter/issues/180#issuecomment-618185664)

##### v4.x Example:

```c#
            string status =
                "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterMediumTest.mp4"), "video/mp4", "tweet_video");

            Media mediaStatusResponse = null;
            do
            {
                if (mediaStatusResponse != null)
                {
                    int checkAfterSeconds = mediaStatusResponse?.ProcessingInfo?.CheckAfterSeconds ?? 0;
                    Console.WriteLine($"Twitter video testing in progress - waiting {checkAfterSeconds} seconds.");
                    await Task.Delay(checkAfterSeconds * 1000);
                }

                mediaStatusResponse =
                    await
                    (from stat in twitterCtx.Media
                     where stat.Type == MediaType.Status &&
                           stat.MediaID == media.MediaID
                     select stat)
                    .SingleOrDefaultAsync(); 
            } while (mediaStatusResponse?.ProcessingInfo?.State == MediaProcessingInfo.InProgress);

            if (mediaStatusResponse?.ProcessingInfo?.State == MediaProcessingInfo.Succeeded)
            {
                Status tweet = await twitterCtx.TweetAsync(status, new ulong[] { media.MediaID });

                if (tweet != null)
                    Console.WriteLine($"Tweet sent: {tweet.Text}");
            }
            else
            {
                MediaError error = mediaStatusResponse?.ProcessingInfo?.Error;

                if (error != null)
                    Console.WriteLine($"Request failed - Code: {error.Code}, Name: {error.Name}, Message: {error.Message}");
            }

```

*Twitter API:* [media/upload](https://developer.twitter.com/en/docs/media/upload-media/api-reference/post-media-upload-init)