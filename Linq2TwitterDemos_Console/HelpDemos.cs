using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class HelpDemos
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
                        Console.WriteLine("\n\tGetting Rate Limits...\n");
                        await GettingRateLimitsAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting configuration...\n");
                        await GetHelpConfigurationAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting languages...\n");
                        await GetHelpLanguagesAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting privacy...\n");
                        await GetPrivacyAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tGetting tos...\n");
                        await GetTosAsync(twitterCtx);
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
            Console.WriteLine("\nHelp Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Rate Limits");
            Console.WriteLine("\t 1. Get Configuration");
            Console.WriteLine("\t 2. Get Languages");
            Console.WriteLine("\t 3. Get Privacy Policy");
            Console.WriteLine("\t 4. Get Terms of Service");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static async Task GettingRateLimitsAsync(TwitterContext twitterCtx)
        {
            var helpResponse =
                await
                    (from help in twitterCtx.Help
                     where help.Type == HelpType.RateLimits
                     select help)
                    .SingleOrDefaultAsync();

            if (helpResponse != null && helpResponse.RateLimits != null)
                foreach (var category in helpResponse.RateLimits)
                {
                    Console.WriteLine("\nCategory: {0}", category.Key);

                    foreach (var limit in category.Value)
                    {
                        Console.WriteLine(
                            "\n  Resource: {0}\n    Remaining: {1}\n    Reset: {2}\n    Limit: {3}",
                            limit.Resource, limit.Remaining, limit.Reset, limit.Limit);
                    }
                }
        }

        static async Task GetHelpConfigurationAsync(TwitterContext twitterCtx)
        {
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Configuration
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null && 
                helpResult.Configuration != null && 
                helpResult.Configuration.NonUserNamePaths != null && 
                helpResult.Configuration.PhotoSizes != null)
            {
                Configuration cfg = helpResult.Configuration;

                Console.WriteLine("Short URL Length: " + cfg.ShortUrlLength);
                Console.WriteLine("Short URL HTTPS Length: " + cfg.ShortUrlLengthHttps);
                Console.WriteLine("Non-UserName Paths: ");
                foreach (var name in cfg.NonUserNamePaths)
                {
                    Console.WriteLine("\t" + name);
                }
                Console.WriteLine("Photo Size Limit: " + cfg.PhotoSizeLimit);
                Console.WriteLine("Max Media Per Upload: " + cfg.MaxMediaPerUpload);
                Console.WriteLine(
                    "Characters Reserved Per Media: " + cfg.CharactersReservedPerMedia);
                Console.WriteLine("Photo Sizes");
                foreach (var photo in cfg.PhotoSizes)
                {
                    Console.WriteLine("\t" + photo.Type);
                    Console.WriteLine("\t\t" + photo.Width);
                    Console.WriteLine("\t\t" + photo.Height);
                    Console.WriteLine("\t\t" + photo.Resize);
                } 
            }
        }

        static async Task GetHelpLanguagesAsync(TwitterContext twitterCtx)
        {
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Languages
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null && helpResult.Languages != null)
                helpResult.Languages.ForEach(lang => 
                    Console.WriteLine("{0}({1}): {2}", lang.Name, lang.Code, lang.Status));
        }

        static async Task GetPrivacyAsync(TwitterContext twitterCtx)
        {
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Privacy
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null)
                Console.WriteLine(helpResult.Policies);
        }

        static async Task GetTosAsync(TwitterContext twitterCtx)
        {
            var helpResult =
                await
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Tos
                 select test)
                .SingleOrDefaultAsync();

            if (helpResult != null)
                Console.WriteLine(helpResult.Policies);
        }
    }
}
