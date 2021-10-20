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
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
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
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Spaces != null)
                searchResponse.Spaces.ForEach(space =>
                    Console.WriteLine(
                        "\n  Creator ID: {0}\n  Date: {1}",
                        space.CreatorID,
                        space.CreatedAt));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
