### Querying Similar Places

Get a list of nearby places with similar names.

*Entity:* [[Geo|Geo Entity]]
*Type:* GeoType.SimilarPlaces

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Attribute | Name/value pair separated by "=" (i.e. "street_address=123 4th Street") | string | no |
| ContainedWithin | PlaceID to restrict results to | string | no |
| Lat | Latitude to search | dec | yes |
| Long | Longitude to search | dec | yes |
| Name | Place name | string | yes |

##### Example:

```c#
            var geoResponse =
                await
                (from g in twitterCtx.Geo
                 where g.Type == GeoType.SimilarPlaces &&
                       g.Latitude == 37.78215 &&
                       g.Longitude == -122.40060 &&
                       g.PlaceName == "Twitter HQ"
                 select g)
                .SingleOrDefaultAsync();

            if (geoResponse != null && geoResponse.Places != null)
                geoResponse.Places.ForEach(place =>
                    Console.WriteLine(
                        "Name: {0}, Country: {1}, Type: {2}",
                        place.Name, place.Country, place.PlaceType));
```

*Twitter API:* [geo/similar_places](https://developer.twitter.com/en/docs/geo/places-near-location/overview)