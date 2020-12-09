Imports LinqToTwitter

Friend Class AccountDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Try
            Dim verifyResponse =
                Await (From acct In twitterCtx.Account
                       Where acct.Type = AccountType.VerifyCredentials
                       Select acct) _
                      .SingleOrDefaultAsync()

            If verifyResponse IsNot Nothing And verifyResponse.User IsNot Nothing Then
                Dim user = verifyResponse.User

                Console.WriteLine(
                        "Credentials are good for {0}.",
                        user.ScreenNameResponse)
            End If

        Catch tqe As Exception
            Console.WriteLine(tqe.Message)
        End Try
    End Function
End Class
