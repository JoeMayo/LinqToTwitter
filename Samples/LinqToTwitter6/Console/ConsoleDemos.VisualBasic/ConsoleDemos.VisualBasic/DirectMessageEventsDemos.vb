Imports LinqToTwitter

Friend Class DirectMessageEventsDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim count = 50 'Set To a low number To demo paging
        Dim cursor = ""
        Dim allDmEvents = New List(Of DMEvent)

        ' you don't have a valid cursor until after the first query
        Dim dmResponse =
            await _
                (from dm in twitterCtx.DirectMessageEvents _
                 where dm.Type = DirectMessageEventsType.List _
                 andalso dm.Count = count _
                 select dm) _
                .SingleOrDefaultAsync()

        allDmEvents.AddRange(dmResponse.Value.DMEvents)
        cursor = dmResponse?.Value?.NextCursor

        while (not string.IsNullOrWhiteSpace(cursor))
            dmResponse =
                await _
                    (from dm in twitterCtx.DirectMessageEvents _
                     where dm.Type = DirectMessageEventsType.List _
                     AndAlso dm.Count = count _
                     andalso dm.Cursor = cursor
                     select dm) _
                    .SingleOrDefaultAsync()

            allDmEvents.AddRange(dmResponse.Value.DMEvents)
            cursor = dmResponse.Value.NextCursor
        End While

        if (not allDmEvents.Any())
            Console.WriteLine("No items returned")
            return
        End If

        Console.WriteLine($"Response Count: {allDmEvents.Count}")
        Console.WriteLine("Responses:")

        allDmEvents.ForEach(
            Sub(evt)
                Dim msgCreate = evt.MessageCreate

                If evt IsNot Nothing AndAlso msgCreate IsNot Nothing Then
                    Console.WriteLine(
                        $"DM ID: {evt.ID}\n" +
                        $"From ID: {msgCreate.SenderID}\n" +
                        $"To ID:  {msgCreate.Target.RecipientID}\n" +
                        $"Message Text: {msgCreate.MessageData.Text}")
                End If
            End Sub)
    End Function
End Class
