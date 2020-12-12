Imports LinqToTwitter

Friend Class BlockDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim blockResponse =
            Await _
                (From block In twitterCtx.Blocks
                 Where block.Type = BlockingType.List
                 Select block) _
                .SingleOrDefaultAsync()

        If blockResponse IsNot Nothing And blockResponse.Users IsNot Nothing Then
            blockResponse.Users.ForEach(
                Sub(user) Console.WriteLine(user.ScreenNameResponse))
        End If
    End Function
End Class
