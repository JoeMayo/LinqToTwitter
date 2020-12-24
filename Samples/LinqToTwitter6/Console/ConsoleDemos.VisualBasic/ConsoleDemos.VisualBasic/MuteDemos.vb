Imports LinqToTwitter

Friend Class MuteDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim muteResponse =
            Await _
            (From mute In twitterCtx.Mute
             Where mute.Type = MuteType.List
             Select mute) _
            .SingleOrDefaultAsync()

        muteResponse?.Users?.ForEach(Sub(user) Console.WriteLine(user.ScreenNameResponse))
    End Function
End Class
