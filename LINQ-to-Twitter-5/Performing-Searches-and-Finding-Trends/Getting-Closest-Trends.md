#### Getting Closest Trends

Get trends closest to a given latitude and longitude.

*Entity:* [[Trends|Trend Entity]]
*Type:* TrendType.Closest

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Latitude | Location latitude | double | yes |
| Longitude | Location longitude | double | yes |

##### v3.0 Example:

```c#
            var trend =
                await
                (from trnd in twitterCtx.Trends
                 where trnd.Type == TrendType.Closest &&
                       trnd.Latitude == 37.78215 &&
                       trnd.Longitude == -122.40060
                 select trnd)
                .SingleOrDefaultAsync();

            if (trend != null && trend.Locations != null)
                trend.Locations.ForEach(
                    loc => Console.WriteLine(
                        "Name: {0}, Country: {1}, WoeID: {2}",
                        loc.Name, loc.Country, loc.WoeID));
```

##### v2.1 Example:

```c#
            var trends =
                from trnd in twitterCtx.Trends
                where trnd.Type == TrendType.Closest &&
                      trnd.Latitude == "37.78215" &&
                      trnd.Longitude == "-122.40060"
                select trnd;

            var trend = trends.SingleOrDefault();

            trend.Locations.ToList().ForEach(
                loc => Console.WriteLine(
                    "Name: {0}, Country: {1}, WoeID: {2}",
                    loc.Name, loc.Country, loc.WoeID));
```

*Twitter API:* [trends/closest](https://developer.twitter.com/en/docs/trends/locations-with-trending-topics/api-reference/get-trends-closest)