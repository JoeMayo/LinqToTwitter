Imports LinqToTwitter

Friend Class GeoDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim geoResponse =
            Await _
                (From geo In twitterCtx.Geo
                 Where geo.Type = GeoType.ID AndAlso
                       geo.ID = "5a110d312052166f"
                 Select geo) _
                .SingleOrDefaultAsync()

        If geoResponse IsNot Nothing AndAlso geoResponse.Places IsNot Nothing Then
            Dim Place = geoResponse.Places.First()

            Console.WriteLine(
                "Name: {0}, Country: {1}, Type: {2}",
                Place.Name, Place.Country, Place.PlaceType)
        End If
    End Function
End Class
