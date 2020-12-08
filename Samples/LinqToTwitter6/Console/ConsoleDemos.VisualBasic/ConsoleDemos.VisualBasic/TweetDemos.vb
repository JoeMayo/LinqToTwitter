Imports LinqToTwitter

Public Class TweetDemos

    Public Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Await twitterCtx.TweetAsync("Test tweet: " & Date.Now)
    End Function

End Class
