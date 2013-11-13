using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class AccountDemos
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
                        Console.WriteLine("\n\tVerifying Credentials...\n");
                        await DoVerifyCredentialsAsync(twitterCtx);
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

        static async Task DoVerifyCredentialsAsync(TwitterContext twitterCtx)
        {
            try
            {
                var verifyResponse =
                    await
                        (from acct in twitterCtx.Account
                         where acct.Type == AccountType.VerifyCredentials
                         select acct)
                        .SingleOrDefaultAsync();
                User user = verifyResponse.User;

                Console.WriteLine("Credentials are good for {0}.", user.ScreenNameResponse);
            }
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(tqe.Message);
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nAccount Demos - Please select:\n");

            Console.WriteLine("\t 0. Verify Credentials");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
