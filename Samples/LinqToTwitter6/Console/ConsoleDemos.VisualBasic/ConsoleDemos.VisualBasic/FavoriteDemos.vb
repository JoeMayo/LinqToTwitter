Imports LinqToTwitter
Imports LinqToTwitter.Common

Friend Class FavoriteDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Const PerQueryFavCount = 200

        ' set from a value that you previously saved
        Dim sinceID = 1

        Dim favsResponse =
            Await _
                (From fav In twitterCtx.Favorites
                 Where fav.Type = FavoritesType.Favorites AndAlso
                 fav.Count = PerQueryFavCount AndAlso
                 fav.TweetMode = TweetMode.Extended
                 Select fav) _
                .ToListAsync()

        If favsResponse Is Nothing Then
            Console.WriteLine("No favorites returned from Twitter.")
            Return
        End If

        Dim favList = New List(Of Favorites)(favsResponse)

        ' first tweet processed on current query
        Dim maxID = favList.Min(Function(fav) fav.StatusID) - 1

        Do
            favsResponse =
                Await _
                    (From fav In twitterCtx.Favorites
                     Where fav.Type = FavoritesType.Favorites AndAlso
                     fav.Count = PerQueryFavCount AndAlso
                     fav.SinceID = sinceID AndAlso
                     fav.MaxID = maxID
                     Select fav) _
                    .ToListAsync()

            If favsResponse Is Nothing Or favsResponse.Count = 0 Then
                Exit Do
            End If

            ' reset first tweet to avoid re-querying the
            ' same list you just received
            maxID = favsResponse.Min(Function(fav) fav.StatusID) - 1
            favList.AddRange(favsResponse)

        Loop While favsResponse.Count > 0

        favList.ForEach(
            Sub(fav)
                If fav IsNot Nothing AndAlso fav.User IsNot Nothing Then
                    Console.WriteLine(
                        "Name: {0}, Tweet: {1}",
                        fav.User.ScreenNameResponse, fav.Text)
                End If
            End Sub)

        ' save this in your db for this user so you can set
        ' sinceID accurately the next time you do a query
        ' And avoid querying the same tweets again.
        Dim newSinceID = favList.Max(Function(fav) fav.SinceID)
    End Function
End Class
