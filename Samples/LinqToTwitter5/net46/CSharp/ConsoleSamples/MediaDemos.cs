using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
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

            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static void PrintTweetsResults(List<Status> tweets)
        {
            if (tweets != null)
                tweets.ForEach(tweet => 
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "ID: [{0}] Name: {1}\n\tTweet: {2}",
                            tweet.StatusID, tweet.User.ScreenNameResponse, tweet.Text);
                });
        }

        static async Task UploadVideoAsync(TwitterContext twitterCtx)
        {
            string status =
                "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            //Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterNormalTest.mp4"), "video/mp4", "tweet_video");
            Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterMediumTest.mp4"), "video/mp4", "tweet_video");
            //Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterErrorTest.mp4"), "video/mp4", "tweet_video");

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
        }

        static async Task CreateMetadataAsync(TwitterContext twitterCtx)
        {
            string status =
                "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            //Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterNormalTest.mp4"), "video/mp4", "tweet_video");
            Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterMediumTest.mp4"), "video/mp4", "tweet_video");
            //Media media = await twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\LinqToTwitterErrorTest.mp4"), "video/mp4", "tweet_video");

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
                await twitterCtx.CreateMediaMetadataAsync(mediaStatusResponse.MediaID, "LINQ to Twitter Alt Text Test");

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
        }
    }
}
