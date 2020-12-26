Imports LinqToTwitter

Friend Class SavedSearchDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim savedSearches =
            Await _
            (From search In twitterCtx.SavedSearch
             Where search.Type = SavedSearchType.Searches
             Select search) _
            .ToListAsync()

        If savedSearches IsNot Nothing Then
            savedSearches.ForEach(
                Sub(search) Console.WriteLine("Search: " + search.Query))
        End If
    End Function
End Class
