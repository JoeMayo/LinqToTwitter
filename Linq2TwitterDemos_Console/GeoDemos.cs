using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class GeoDemos
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
                        Console.WriteLine("\n\tSearching by IP...\n");
                        await SearchByIP(twitterCtx);
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

        static async Task SearchByIP(TwitterContext twitterCtx)
        {
            var geoResponse =
                await
                    (from geo in twitterCtx.Geo
                     where geo.Type == GeoType.Search &&
                           geo.IP == "168.143.171.180"
                     select geo)
                    .SingleOrDefaultAsync();

            Place place = geoResponse.Places.First();

            Console.WriteLine(
                "Name: {0}, Country: {1}, Type: {2}",
                place.Name, place.Country, place.PlaceType);
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nGeo Demos - Please select:\n");

            Console.WriteLine("\t 0. Search by IP");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
