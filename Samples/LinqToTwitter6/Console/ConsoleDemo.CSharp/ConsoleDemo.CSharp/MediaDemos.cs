using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    class MediaDemos
    {
        internal static async Task RunAsync(TwitterContext twitterCtx)
        {
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tUploading a video...\n");
                        await UploadVideoAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tCreating metadata...\n");
                        await CreateMetadataAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tUploading single image...\n");
                        await UploadSingleImageAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tUploading multiple images...\n");
                        await UploadMultipleImagesAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nReturning...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nMedia Demos - Please select:\n");

            Console.WriteLine("\t 0. Upload a Video");
            Console.WriteLine("\t 1. Create Metadata");
            Console.WriteLine("\t 2. Upload a Single Image");
            Console.WriteLine("\t 3. Upload Muliple Images");

            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task UploadVideoAsync(TwitterContext twitterCtx)
        {
            string text =
                "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            byte[] imageBytes = File.ReadAllBytes(@"..\..\..\images\TwitterTest.mp4");
            const string JoeMayoUserID = "15411837";
            var taggedUserIds = new string[] { JoeMayoUserID };
            string mediaType = "video/mp4";
            string mediaCategory = "tweet_video";

            Media? media = await twitterCtx.UploadMediaAsync(imageBytes, mediaType, mediaCategory);

            if (media == null)
            {
                Console.WriteLine("Invalid Media returned from UploadMediaAsync");
                return;
            }

            Media? mediaStatusResponse = null;
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
                Tweet? tweet = await twitterCtx.TweetMediaAsync(text, new List<string> { media.MediaID.ToString() }, taggedUserIds);

                if (tweet != null)
                    Console.WriteLine($"Tweet sent: {tweet.Text}");
            }
            else
            {
                MediaError? error = mediaStatusResponse?.ProcessingInfo?.Error;

                if (error != null)
                    Console.WriteLine($"Request failed - Code: {error.Code}, Name: {error.Name}, Message: {error.Message}");
            }
        }

        static async Task CreateMetadataAsync(TwitterContext twitterCtx)
        {
            string text =
                "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            byte[] imageBytes = File.ReadAllBytes(@"..\..\..\images\TwitterTest.mp4");
            const string JoeMayoUserID = "15411837";
            var taggedUserIds = new string[] { JoeMayoUserID };
            string mediaType = "video/mp4";
            string mediaCategory = "tweet_video";

            Media? media = await twitterCtx.UploadMediaAsync(imageBytes, mediaType, mediaCategory);

            if (media == null)
            {
                Console.WriteLine("Invalid Media returned from UploadMediaAsync");
                return;
            }

            Media? mediaStatusResponse = null;
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
                await twitterCtx.CreateMediaMetadataAsync(mediaStatusResponse.MediaID, "LINQ to Twitter Alt Text Test");

                Tweet? tweet = await twitterCtx.TweetMediaAsync(text, new List<string> { media.MediaID.ToString() }, taggedUserIds);

                if (tweet != null)
                    Console.WriteLine($"Tweet sent: {tweet.Text}");
            }
            else
            {
                MediaError? error = mediaStatusResponse?.ProcessingInfo?.Error;

                if (error != null)
                    Console.WriteLine($"Request failed - Code: {error.Code}, Name: {error.Name}, Message: {error.Message}");
            }
        }

        static async Task UploadSingleImageAsync(TwitterContext twitterCtx)
        {
            var taggedUserIds = new List<string> { "3265644348", "15411837" };
            string text =
                "Testing single-image tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string mediaCategory = "tweet_image";

            Media? media = await twitterCtx.UploadMediaAsync(
                File.ReadAllBytes(@"..\..\..\images\200xColor_2.png"),
                "image/png",
                mediaCategory);

            if (media == null)
            {
                Console.WriteLine("Problem uploading media.");
                return;
            }

            Tweet? tweet = await twitterCtx.TweetMediaAsync(text, new List<string> { media.MediaID.ToString() }, taggedUserIds);

            if (tweet != null)
                Console.WriteLine("Tweet sent: " + tweet.Text);
        }

        static async Task UploadMultipleImagesAsync(TwitterContext twitterCtx)
        {
            var taggedUserIds = new List<string> { "3265644348", "15411837" };
            string text =
                "Testing multi-image tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string mediaCategory = "tweet_image";

            var imageUploadTasks =
                new List<Task<Media?>>
                {
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\..\images\200xColor_2.png"), "image/png", mediaCategory),
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\..\images\WP_000003.jpg"), "image/jpg", mediaCategory),
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\..\images\13903749474_86bd1290de_o.jpg"), "image/jpg", mediaCategory),
                };

            await Task.WhenAll(imageUploadTasks);

            List<string> mediaIds =
                (from tsk in imageUploadTasks
                 select tsk.Result.MediaID.ToString())
                .ToList();

            Tweet? tweet = await twitterCtx.TweetMediaAsync(text, mediaIds, taggedUserIds);

            if (tweet != null)
                Console.WriteLine($"Tweet sent: {tweet.Text}");
        }
    }
}
