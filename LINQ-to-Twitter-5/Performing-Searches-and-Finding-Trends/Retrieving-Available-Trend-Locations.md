#### Retrieving Available Trend Locations

Get locations where trends are occurring.

*Entity:* [[Trends|Trend Entity]]
*Type:* TrendType.Available

##### Parameters/Filters:

| Name | Purpose | Type | Required |
|------|---------|------|----------|
| Lat | Latitude coordinate | dec | yes |
| Long | Longitude coordinate | dec | yes |

##### v3.0 Example:

```c#
            var trendsResponse =
                await
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Available
                 select trend)
                .SingleOrDefaultAsync();

            if (trendsResponse != null && trendsResponse.Locations != null)
                trendsResponse.Locations.ForEach(
                    loc => Console.WriteLine("Location: " + loc.Name));
```

##### v2.1 Example:

```c#
            var trends =
                from trnd in twitterCtx.Trends
                where trnd.Type == TrendType.Available
                select trnd;

            var trend = trends.FirstOrDefault();

            trend.Locations.ToList().ForEach(
                loc => Console.WriteLine(
                    "Name: {0}, Country: {1}, WoeID: {2}",
                    loc.Name, loc.Country, loc.WoeID));
```

*Twitter API:* [trends/available](https://developer.twitter.com/en/docs/trends/locations-with-trending-topics/api-reference/get-trends-available)