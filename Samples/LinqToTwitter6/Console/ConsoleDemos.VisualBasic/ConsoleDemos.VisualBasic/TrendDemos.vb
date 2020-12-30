Imports LinqToTwitter

Friend Class TrendDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim trendsResponse =
            Await _
            (From trend In twitterCtx.Trends
             Where trend.Type = TrendType.Available
             Select trend) _
            .SingleOrDefaultAsync()

        If trendsResponse IsNot Nothing AndAlso trendsResponse.Locations IsNot Nothing Then
            trendsResponse.Locations.ForEach(
                Sub(loc) Console.WriteLine("Location: " + loc.Name))
        End If
    End Function
End Class
