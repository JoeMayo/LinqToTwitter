Imports LinqToTwitter
Imports LinqToTwitter.Common

Friend Class UserDemos
    Friend Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
        Dim userResponse =
                Await _
                (From user In twitterCtx.TwitterUser
                 Where user.Type = UserType.UsernameLookup AndAlso
                       user.Usernames = "JoeMayo,Linq2Twitr" AndAlso
                       user.Expansions = ExpansionField.AllUserFields AndAlso
                       user.TweetFields = TweetField.AllFieldsExceptPermissioned AndAlso
                       user.UserFields = UserField.AllFields
                 Select user) _
                .SingleOrDefaultAsync()

        If (userResponse IsNot Nothing) Then
            userResponse.Users.ForEach(
                Sub(user)
                    Console.WriteLine("Name: " + user.Username)
                End Sub)
        End If
    End Function
End Class
