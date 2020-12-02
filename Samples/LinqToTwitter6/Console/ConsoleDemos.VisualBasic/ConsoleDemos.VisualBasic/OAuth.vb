Imports LinqToTwitter.OAuth

Public Class OAuth
    Function ChooseAuthenticationStrategy() As IAuthorizer
        Console.WriteLine("Authentication Strategy:\n\n")

        Console.WriteLine("  1 - Pin (default)")
        Console.WriteLine("  2 - Application-Only")
        Console.WriteLine("  3 - Single User")
        Console.WriteLine("  4 - XAuth")

        Console.Write("\nPlease choose (1, 2, 3, or 4): ")
        Dim Input As ConsoleKeyInfo = Console.ReadKey()
        Console.WriteLine("")

        Dim auth As IAuthorizer

        Select Case Input.Key
            Case ConsoleKey.D1
                auth = DoPinOAuth()
            Case ConsoleKey.D2
                auth = DoApplicationOnlyAuth()
            Case ConsoleKey.D3
                auth = DoSingleUserAuth()
            Case ConsoleKey.D4
                auth = DoXAuth()
            Case Else
                auth = DoPinOAuth()

        End Select

        Return auth
    End Function

    Private Function DoXAuth() As IAuthorizer
        Throw New NotImplementedException()
    End Function

    Private Function DoSingleUserAuth() As IAuthorizer
        Throw New NotImplementedException()
    End Function

    Private Function DoApplicationOnlyAuth() As IAuthorizer
        Throw New NotImplementedException()
    End Function

    Private Function DoPinOAuth() As IAuthorizer
        Throw New NotImplementedException()
    End Function
End Class
