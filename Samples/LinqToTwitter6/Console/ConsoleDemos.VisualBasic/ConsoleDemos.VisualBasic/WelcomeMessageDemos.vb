Imports LinqToTwitter

Friend Class WelcomeMessageDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim message =
                Await _
                (From welcomeMsg In twitterCtx.WelcomeMessage
                 Where welcomeMsg.Type = WelcomeMessageType.ListMessages
                 Select welcomeMsg) _
                .SingleOrDefaultAsync()

        Dim allWelcomeMessages = message.Value.WelcomeMessages

        If allWelcomeMessages Is Nothing Then
            Console.WriteLine("No welcome messages found.")
            Return
        End If

        Console.WriteLine($"Response Count: {allWelcomeMessages.Count}")
        Console.WriteLine("Responses:")

        allWelcomeMessages.ForEach(
            Sub(msg)
                If msg IsNot Nothing Then
                    Console.WriteLine(
                        $"Message ID: '{msg.Id}' \n" +
                        $"      Name: '{msg.Name} \n" +
                        $"      Text: '{msg.MessageData?.Text}\n")
                End If
            End Sub)
    End Function
End Class
