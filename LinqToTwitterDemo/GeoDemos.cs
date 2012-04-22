using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows geo demos
    /// </summary>
    public class GeoDemos
    {
        /// <summary>
        /// Run all geo related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            LookupReverseGeocodeDemo(twitterCtx);
            //LookupGeoIDDemo(twitterCtx);
            //SearchDemo(twitterCtx);
        }

        /// <summary>
        /// Shows how to perform a search query to 
        /// find a place, based on IP address
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchDemo(TwitterContext twitterCtx)
        {
            var geo =
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.Search &&
                       g.IP == "168.143.171.180"
                 select g)
                 .FirstOrDefault();

            Place place = geo.Places[0];

            Console.WriteLine(
                "Name: {0}, Country: {1}, Type: {2}",
                place.Name, place.Country, place.PlaceType);
        }

        /// <summary>
        /// Shows how to perform a reverse geocode lookup
        /// to find a place, based on latitude and longitude
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void LookupGeoIDDemo(TwitterContext twitterCtx)
        {
            var geo =
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.ID &&
                       g.ID == "5a110d312052166f"
                 select g)
                 .SingleOrDefault();

            Place place = geo.Places[0];

            Console.WriteLine(
                "Name: {0}, Country: {1}, Type: {2}",
                place.Name, place.Country, place.PlaceType);
        }

        /// <summary>
        /// Shows how to perform a reverse geocode lookup
        /// to find a place, based on latitude and longitude
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void LookupReverseGeocodeDemo(TwitterContext twitterCtx)
        {
            var geo =
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.Reverse &&
                       g.Latitude == 37.78215 &&
                       g.Longitude == -122.40060
                 select g)
                 .SingleOrDefault();

            geo.Places.ForEach(
                place =>
                    Console.WriteLine(
                        "Name: {0}, Country: {1}, Type: {2}",
                        place.Name, place.Country, place.PlaceType));
        }
    }
}
