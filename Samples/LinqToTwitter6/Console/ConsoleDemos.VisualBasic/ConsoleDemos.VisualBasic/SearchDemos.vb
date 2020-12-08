Imports LinqToTwitter
Imports LinqToTwitter.Common

Public Class SearchDemos
    Public Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim Response As Search =
            Await (From search In twitterCtx.Search()
                   Where search.Type = SearchType.Search _
                   AndAlso search.TweetMode = TweetMode.Extended _
                   AndAlso search.Query = "Coronavirus") _
                  .SingleOrDefaultAsync()

        Dim tweets As List(Of Status) = Response.Statuses()

        If Response IsNot Nothing AndAlso Response.Statuses IsNot Nothing Then
            For Each str As Status In tweets
                Console.WriteLine(str.StatusID.ToString() + " " + str.FullText)

                If str.ExtendedEntities.MediaEntities.Count > 0 Then
                    Console.WriteLine(" - Media URL: " + str.ExtendedEntities.MediaEntities(0).MediaUrl)
                End If
            Next
        End If
    End Function
End Class
