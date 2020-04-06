using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    public class MuteDemos
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
                        Console.WriteLine("\n\tLooking for IDs...\n");
                        await LookupIDsAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tLooking for Users...\n");
                        await LookupUsersAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tMuting...\n");
                        await MuteUserAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tShowing...\n");
                        await UnmuteUserAsync(twitterCtx);
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
            Console.WriteLine("\nUser Demos - Please select:\n");

            Console.WriteLine("\t 0. Lookup Muted User IDs");
            Console.WriteLine("\t 1. Lookup Muted Users");
            Console.WriteLine("\t 2. Mute User");
            Console.WriteLine("\t 3. Unmute User");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LookupIDsAsync(TwitterContext twitterCtx)
        {
            var muteResponse =
                await
                (from mute in twitterCtx.Mute
                 where mute.Type == MuteType.IDs
                 select mute)
                .SingleOrDefaultAsync();

            muteResponse.IDList.ForEach(id => Console.WriteLine(id));
        }

        static async Task LookupUsersAsync(TwitterContext twitterCtx)
        {
            var muteResponse =
                await
                (from mute in twitterCtx.Mute
                 where mute.Type == MuteType.List
                 select mute)
                .SingleOrDefaultAsync();

            muteResponse.Users.ForEach(user => Console.WriteLine(user.ScreenNameResponse));
        }

        static async Task MuteUserAsync(TwitterContext twitterCtx)
        {
            const string ScreenName = "justinbieber";

            User mutedUser = await twitterCtx.MuteAsync(ScreenName);

            Console.WriteLine("You muted {0}", mutedUser.ScreenNameResponse);
        }

        static async Task UnmuteUserAsync(TwitterContext twitterCtx)
        {
            const string ScreenName = "JoeMayo";

            User unmutedUser = await twitterCtx.UnMuteAsync(ScreenName);

            Console.WriteLine("You un-muted {0}", unmutedUser.ScreenNameResponse);
        }
    }
}
