using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace ConsoleDemo.CSharp
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
                        Console.WriteLine("\n\tLooking for muted users...\n");
                        await LookupMutesAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tMuting...\n");
                        await MuteUserAsync(twitterCtx);
                        break;
                    case '2':
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
            Console.WriteLine("\t 1. Mute User");
            Console.WriteLine("\t 2. Unmute User");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LookupMutesAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            var muteResponse =
                await
                (from mute in twitterCtx.Mute
                 where mute.Type == MuteType.Muted &&
                       mute.ID == userID
                 select mute)
                .SingleOrDefaultAsync();

            muteResponse?.Users?.ForEach(
                user => Console.WriteLine($"{user.ID}: {user.Username} - {user.Name}"));
        }

        static async Task MuteUserAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Mute: ");
            string? userName = Console.ReadLine() ?? "";

            TwitterUserQuery? userResponse =
                await
                (from usr in twitterCtx.TwitterUser
                 where usr.Type == UserType.UsernameLookup &&
                       usr.Usernames == userName
                 select usr)
                .SingleOrDefaultAsync();

            string? targetUserID = userResponse?.Users?.FirstOrDefault()?.ID;
            string? sourceUserID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (targetUserID == null || sourceUserID == null)
            {
                Console.WriteLine($"Either {nameof(targetUserID)} or {nameof(sourceUserID)} is null.");
                return;
            }

            MuteResponse? muted = await twitterCtx.MuteAsync(sourceUserID, targetUserID);

            if (muted?.Data != null)
                Console.WriteLine("Is Muted: " + muted.Data.Muting);
        }

        static async Task UnmuteUserAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Unmute: ");
            string? userName = Console.ReadLine() ?? "";

            TwitterUserQuery? userResponse =
                await
                (from usr in twitterCtx.TwitterUser
                 where usr.Type == UserType.UsernameLookup &&
                       usr.Usernames == userName
                 select usr)
                .SingleOrDefaultAsync();

            string? targetUserID = userResponse?.Users?.FirstOrDefault()?.ID;
            string? sourceUserID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (targetUserID == null || sourceUserID == null)
            {
                Console.WriteLine($"Either {nameof(targetUserID)} or {nameof(sourceUserID)} is null.");
                return;
            }

            MuteResponse? muted = await twitterCtx.UnMuteAsync(sourceUserID, targetUserID);

            if (muted?.Data != null)
                Console.WriteLine("Is Muted: " + muted.Data.Muting);
        }
    }
}
