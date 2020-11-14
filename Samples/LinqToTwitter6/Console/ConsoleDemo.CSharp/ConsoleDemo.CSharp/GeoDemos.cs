using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace ConsoleDemo.CSharp
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
                        await SearchByIPAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tLooking for geo...\n");
                        await LookupGeoIDAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tFinding reverse geocode...\n");
                        await LookupReverseGeocodeAsync(twitterCtx);
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
            Console.WriteLine("\nGeo Demos - Please select:\n");

            Console.WriteLine("\t 0. Search by IP");
            Console.WriteLine("\t 1. Lookup Geo by ID");
            Console.WriteLine("\t 2. Lookup Reverse Geocode");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task SearchByIPAsync(TwitterContext twitterCtx)
        {
            var geoResponse =
                await
                    (from geo in twitterCtx.Geo
                     where geo.Type == GeoType.Search &&
                           geo.IP == "74.125.19.104"
                     select geo)
                    .SingleOrDefaultAsync();

            if (geoResponse != null && geoResponse.Places != null)
            {
                Place place = geoResponse.Places.First();

                Console.WriteLine(
                    "Name: {0}, Country: {1}, Type: {2}",
                    place.Name, place.Country, place.PlaceType); 
            }
        }

        static async Task LookupGeoIDAsync(TwitterContext twitterCtx)
        {
            var geoResponse =
                await
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.ID &&
                       g.ID == "5a110d312052166f"
                 select g)
                .SingleOrDefaultAsync();

            if (geoResponse != null && geoResponse.Places != null)
            {
                Place place = geoResponse.Places.First();

                Console.WriteLine(
                    "Name: {0}, Country: {1}, Type: {2}",
                    place.Name, place.Country, place.PlaceType);
            }
        }

        static async Task LookupReverseGeocodeAsync(TwitterContext twitterCtx)
        {
            var geoResponse =
                await
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.Reverse &&
                       g.Latitude == 37.78215 &&
                       g.Longitude == -122.40060
                 select g)
                .SingleOrDefaultAsync();

            if (geoResponse != null && geoResponse.Places != null)
                geoResponse.Places.ForEach(place =>
                    Console.WriteLine(
                        "Name: {0}, Country: {1}, Type: {2}",
                        place.Name, place.Country, place.PlaceType));
        }
    }
}
