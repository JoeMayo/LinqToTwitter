### Searching for Places

Search for places that can be attached to a status update.

*Entity:* [Geo](../LINQ-to-Twitter-Entities/Geo-Entity.md)

*Type:* GeoType.Search

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Accuracy | Radius to search | string | no |
| Attribute | Name/value pair separated by "=" (i.e. "street_address=123 4th Street") | string | no |
| ContainedWithin | PlaceID to restrict results to | string | no |
| Granularity | Minimum granularity of place types | string | no |
| IP | IP address of user | string | only if Lat, Long, and Query are empty |
| Lat | Latitude to search | dec | only if IP and Query are empty |
| Long | Longitude to search | dec | only if IP and Query are empty |
| MaxResults | Max nearby results | int | no |
| Query | URL encoded text search term | string | only if Lat, Long, and IP are empty |

##### Example:

```c#
            var geoResponse =
                await
                    (from geo in twitterCtx.Geo
                     where geo.Type == GeoType.Search &&
                           geo.IP == "168.143.171.180"
                     select geo)
                    .SingleOrDefaultAsync();

            if (geoResponse != null && geoResponse.Places != null)
            {
                Place place = geoResponse.Places.First();

                Console.WriteLine(
                    "Name: {0}, Country: {1}, Type: {2}",
                    place.Name, place.Country, place.PlaceType); 
            }
```

*Twitter API:* [geo/search](https://developer.twitter.com/en/docs/geo/places-near-location/api-reference/get-geo-search)