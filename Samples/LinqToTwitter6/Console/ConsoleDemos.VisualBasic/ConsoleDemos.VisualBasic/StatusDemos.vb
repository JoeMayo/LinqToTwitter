Imports LinqToTwitter
Imports LinqToTwitter.Common

Friend Class StatusDemos

    Shared Sub PrintTweetsResults(tweets As List(Of Status))
        If tweets IsNot Nothing Then
            tweets.ForEach(
                Sub(tweet)
                    If tweet IsNot Nothing AndAlso tweet.User IsNot Nothing Then
                        Console.WriteLine(
                            "ID: [{0}] Name: {1}" + Environment.NewLine + "    Tweet: {2}",
                            tweet.StatusID, tweet.User.ScreenNameResponse,
                            IIf(String.IsNullOrWhiteSpace(tweet.Text), tweet.FullText, tweet.Text))
                    End If
                End Sub)
        End If
    End Sub

    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim tweets =
            Await _
            (From tweet In twitterCtx.Status
             Where tweet.Type = StatusType.Home AndAlso
                   tweet.TweetMode = TweetMode.Extended AndAlso
                   tweet.Count = 150
             Select tweet) _
            .ToListAsync()

        PrintTweetsResults(tweets)
    End Function
End Class
