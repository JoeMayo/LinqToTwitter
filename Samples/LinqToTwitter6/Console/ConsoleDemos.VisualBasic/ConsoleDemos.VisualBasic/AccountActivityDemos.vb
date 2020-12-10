Imports LinqToTwitter

Friend Class AccountActivityDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim webhooksResponse =
                Await _
                (From acct In twitterCtx.AccountActivity
                 Where acct.Type = AccountActivityType.Webhooks
                 Select acct) _
                .SingleOrDefaultAsync()

        If webhooksResponse.WebhooksValue.Webhooks IsNot Nothing Then
            Console.WriteLine("Webhooks:")

            If (webhooksResponse.WebhooksValue.Webhooks.Any()) Then
                For Each webhook In webhooksResponse.WebhooksValue.Webhooks
                    Console.WriteLine(
                        $"ID: {webhook.ID}, " +
                        $"Created: {webhook.CreatedTimestamp}, " +
                        $"Valid: {webhook.Valid}, " +
                        $"URL: {webhook.Url}")
                Next
            Else
                Console.WriteLine("No webhooks registered")
            End If
        End If
    End Function
End Class
