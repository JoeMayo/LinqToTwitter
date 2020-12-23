Imports LinqToTwitter

Friend Class ListDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim screenName = "Linq2Twitr"

        Dim lists =
            Await _
            (From list In twitterCtx.List
             Where list.Type = ListType.List AndAlso
                   list.ScreenName = screenName
             Select list) _
            .ToListAsync()

        If lists IsNot Nothing Then
            lists.ForEach(Sub(list) Console.WriteLine("Slug: " + list.SlugResponse))
        End If
    End Function
End Class
