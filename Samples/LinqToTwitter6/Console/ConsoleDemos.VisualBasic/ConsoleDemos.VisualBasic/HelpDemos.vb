Imports LinqToTwitter

Friend Class HelpDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim helpResponse =
            Await _
                (From help In twitterCtx.Help
                 Where help.Type = HelpType.RateLimits
                 Select help) _
                .SingleOrDefaultAsync()

        If helpResponse IsNot Nothing AndAlso helpResponse.RateLimits IsNot Nothing Then
            For Each Category In helpResponse.RateLimits
                Console.WriteLine("{1}Category: {0}", Category.Key, Environment.NewLine)

                For Each limit In Category.Value
                    Console.WriteLine(
                        "{4}  Resource: {0}{4}    Remaining: {1}{4}    Reset: {2}{4}    Limit: {3}",
                        limit.Resource, limit.Remaining, limit.Reset, limit.Limit, Environment.NewLine)
                Next
            Next
        End If
    End Function
End Class
