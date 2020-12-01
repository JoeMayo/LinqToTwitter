### Getting Details on a Place

With a place ID, returned by a Reverse Geocode lookup, you can get more details on the place.

*Entity:* [[Geo|Geo Entity]]
*Type:* GeoType.ID

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| PlaceID | ID retrieved from reverse geocode | string | yes |

##### Example:

```c#
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
```

*Twitter API:* [geo/id/:place_id ](https://developer.twitter.com/en/docs/geo/place-information/api-reference/get-geo-id-place_id)