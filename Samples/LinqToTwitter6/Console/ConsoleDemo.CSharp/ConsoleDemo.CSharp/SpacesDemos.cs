using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    public class SpacesDemos
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
                        Console.WriteLine("\n\tSearching...\n");
                        await DoSearchAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting by creator IDs...\n");
                        await DoByCreatorIdsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting by space IDs...\n");
                        await DoBySpaceIdsAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting space buyers...");
                        await DoSpaceBuyerSearchAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tGetting space tweets...");
                        await DoSpaceTweetsSearchAsync(twitterCtx);
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
            Console.WriteLine("\nSearch Demos - Please select:\n");

            Console.WriteLine("\t 0. Search");
            Console.WriteLine("\t 1. Get by Creator IDs");
            Console.WriteLine("\t 2. Get by Space IDs");
            Console.WriteLine("\t 3. Get space buyers");
            Console.WriteLine("\t 4. Get space tweets");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoByCreatorIdsAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please enter one or more creator (user) IDs (comma-separated): ");
            string? creatorIds = Console.ReadLine();

            SpacesQuery? searchResponse =
                await
                (from search in twitterCtx.Spaces
                 where search.Type == SpacesType.ByCreatorID &&
                       search.CreatorIds == creatorIds &&
                       search.Expansions == ExpansionField.AllSpaceFields &&
                       search.SpaceFields == SpaceField.AllFields &&
                       search.TopicFields == TopicField.AllFields &&
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Spaces != null)
                searchResponse.Spaces.ForEach(space =>
                    Console.WriteLine(
                        "\n  Creator ID: {0}" +
                        "\n  Space ID: {1}" +
                        "\n  Date: {2}",
                        space.CreatorID,
                        space.ID,
                        space.CreatedAt));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoBySpaceIdsAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please enter one or more space IDs (comma-separated): ");
            string? spaceIds = Console.ReadLine();

            SpacesQuery? searchResponse =
                await
                (from search in twitterCtx.Spaces
                 where search.Type == SpacesType.BySpaceID &&
                       search.SpaceIds == spaceIds &&
                       search.Expansions == ExpansionField.AllSpaceFields &&
                       search.SpaceFields == SpaceField.AllFields &&
                       search.TopicFields == TopicField.AllFields &&
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Spaces != null)
                searchResponse.Spaces.ForEach(space =>
                    Console.WriteLine(
                        "\n  Creator ID: {0}" +
                        "\n  Space ID: {1}" +
                        "\n  Date: {2}",
                        space.CreatorID,
                        space.ID,
                        space.CreatedAt));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "twitter";
            //searchTerm = "кот (";

            SpacesQuery? searchResponse =
                await
                (from search in twitterCtx.Spaces
                 where search.Type == SpacesType.Search &&
                       search.Query == searchTerm &&
                       search.MaxResults == 100 &&
                       search.Expansions == ExpansionField.AllSpaceFields &&
                       search.SpaceFields == SpaceField.AllFields &&
                       search.State == SpaceState.All &&
                       search.TopicFields == TopicField.AllFields &&
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Spaces != null)
                searchResponse.Spaces.ForEach(space =>
                    Console.WriteLine(
                        "\n  Creator ID: {0}" +
                        "\n  Space ID: {1}" +
                        "\n  Date: {2}",
                        space.CreatorID,
                        space.ID,
                        space.CreatedAt));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoSpaceBuyerSearchAsync(TwitterContext twitterCtx)
        {
            TwitterUserQuery? searchResponse =
                await
                (from space in twitterCtx.TwitterUser
                 where space.Type == UserType.SpaceBuyers &&
                       space.SpaceID == "1DXxyRYNejbKM" &&
                       space.Expansions == ExpansionField.AllTweetFields &&
                       space.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       space.PlaceFields == PlaceField.AllFields &&
                       space.PollFields == PollField.AllFields &&
                       space.UserFields == UserField.AllFields &&
                       space.TweetFields == TweetField.AllFieldsExceptPermissioned
                 select space)
                .SingleOrDefaultAsync();

            if (searchResponse != null)
                searchResponse.Users?.ForEach(user =>
                    Console.WriteLine("Name: " + user.Username));
        }

        static async Task DoSpaceTweetsSearchAsync(TwitterContext twitterCtx)
        {
            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.SpaceTweets &&
                       tweet.SpaceID == "1DXxyRYNejbKM" &&
                       tweet.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       tweet.Expansions == ExpansionField.AllTweetFields &&
                       tweet.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       tweet.PlaceFields == PlaceField.AllFields &&
                       tweet.PollFields == PollField.AllFields &&
                       tweet.UserFields == UserField.AllFields
                 select tweet)
                .SingleOrDefaultAsync();

            if (tweetResponse?.Tweets != null)
                tweetResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nID: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
