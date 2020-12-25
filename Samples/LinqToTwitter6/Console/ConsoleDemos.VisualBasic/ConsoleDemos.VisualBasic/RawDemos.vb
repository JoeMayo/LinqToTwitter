Imports LinqToTwitter

Friend Class RawDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        If twitterCtx Is Nothing Then
            Throw New ArgumentNullException(NameOf(twitterCtx))
        End If

        Dim unencodedStatus = "JoeMayo"
        Dim encodedStatus = Uri.EscapeDataString(unencodedStatus)
        Dim queryString = "tweets/search/recent?query=" + encodedStatus

        Dim previousBaseUrl = twitterCtx.BaseUrl
        twitterCtx.BaseUrl = "https://api.twitter.com/2/"

        Dim rawResult =
            Await _
            (From raw In twitterCtx.RawQuery
             Where raw.QueryString = queryString
             Select raw) _
            .SingleOrDefaultAsync()

        If rawResult IsNot Nothing Then
            Console.WriteLine(
                "Response from Twitter: " + rawResult.Response)
        End If

        twitterCtx.BaseUrl = previousBaseUrl
    End Function
End Class
