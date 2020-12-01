### Performing Reverse Geocode Lookups

Lets you provide latitude and longitude to get back location/place information.

*Entity:* [[Geo|Geo Entity]]
*Type:* GeoType.Reverse

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Accuracy | Radius to search | string | no |
| Granularity | Minimum granularity of place types | string | no |
| Lat | Latitude to search | dec | yes |
| Long | Longitude to search | dec | yes |
| MaxResults | Max nearby results | int | no |

##### Example:

```c#
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
```

*Twitter API:* [geo/reverse_geocode](https://developer.twitter.com/en/docs/geo/places-near-location/api-reference/get-geo-reverse_geocode)