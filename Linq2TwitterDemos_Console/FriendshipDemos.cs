using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class FriendshipDemos
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
                        Console.WriteLine("\n\tShowing friends...\n");
                        await ShowFriends(twitterCtx);
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

        static async Task ShowFriends(TwitterContext twitterCtx)
        {
            var friendshipResponse =
                await
                    (from friend in twitterCtx.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == "JoeMayo"
                     select friend)
                    .SingleOrDefaultAsync();

            friendshipResponse.Users.ForEach(friend => 
                Console.WriteLine(
                    "Name: {0}",
                    friend.ScreenNameResponse));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nFriendship Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Friends");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
