﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    class LikeDemos
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
                        Console.WriteLine("\n\tLiking tweet...\n");
                        await LikeAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tUnliking tweet...\n");
                        await UnlikeAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tLooking for Likes...\n");
                        await LookupLikesAsync(twitterCtx);
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
            Console.WriteLine("\nLike Demos - Please select:\n");

            Console.WriteLine("\t 0. Like a Tweet");
            Console.WriteLine("\t 1. Unlike a Tweet");
            Console.WriteLine("\t 2. Lookup Likes");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LikeAsync(TwitterContext twitterCtx)
        {
            string tweetID = "1371844879043723273";
            string? userID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (userID == null)
            {
                Console.WriteLine($"{nameof(userID)} is null.");
                return;
            }

            LikedResponse? user = await twitterCtx.LikeAsync(userID, tweetID);

            if (user?.Data != null)
                Console.WriteLine("Is Liked: " + user.Data.Liked);
        }

        static async Task UnlikeAsync(TwitterContext twitterCtx)
        {
            string? tweetID = "1371844879043723273";
            string? userID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (userID == null)
            {
                Console.WriteLine($"{nameof(userID)} is null.");
                return;
            }

            LikedResponse? user = await twitterCtx.UnlikeAsync(userID, tweetID);

            if (user?.Data != null)
                Console.WriteLine("Is Liked: " + user.Data.Liked);
        }

        static async Task LookupLikesAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            LikeQuery? likeResponse =
                await
                    (from like in twitterCtx.Likes
                     where
                        like.Type == LikeType.Lookup &&
                        like.ID == userID &&
                        like.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                        like.PlaceFields == PlaceField.AllFields &&
                        like.PollFields == PollField.AllFields &&
                        like.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                        like.UserFields == UserField.AllFields
                     select like)
                    .SingleOrDefaultAsync();

            if (likeResponse != null && likeResponse.Tweets != null)
                likeResponse.Tweets.ForEach(tweet =>
                        Console.WriteLine(tweet.Text));
        }
    }
}
