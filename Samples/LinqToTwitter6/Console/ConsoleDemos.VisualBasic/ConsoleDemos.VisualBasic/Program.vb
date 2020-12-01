Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports LinqToTwitter
Imports LinqToTwitter.Common
Imports LinqToTwitter.OAuth

Module Program

    Sub Main()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        SendTweet().Wait()
    End Sub

    Async Function SendTweet() As Task
        Dim credentials As InMemoryCredentialStore = New InMemoryCredentialStore With {
            .ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
            .ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
        }

        Dim auth As PinAuthorizer = New PinAuthorizer()
        auth.CredentialStore = credentials
        auth.GetPin =
                Function()
                    Console.WriteLine("Next, you'll need to tell Twitter to authorize access. This program will not have access to your credentials, which is the benefit of OAuth. Once you log into Twitter and give this program permission, come back to this console.")
                    Console.WriteLine()
                    Console.Write("Please enter the PIN from Twitter: ")

                    Return Console.ReadLine()
                End Function
        auth.GoToTwitterAuthorization =
            Function(pageLink)
                Dim psi As ProcessStartInfo = New ProcessStartInfo
                With psi
                    .FileName = pageLink
                    .UseShellExecute = True
                End With
                Return Process.Start(psi)
            End Function

        Await auth.AuthorizeAsync()

        Dim twitterCtx = New TwitterContext(auth)

        Console.Write("Type '1' to tweet, '2' to search, or '3' to upload a video: ")
        Dim key = Console.ReadKey()

        If (key.Key = ConsoleKey.D1) Then
            Await twitterCtx.TweetAsync("Test tweet: " & Date.Now)
        ElseIf (key.Key = ConsoleKey.D2) Then
            Await SearchAsync(twitterCtx)
        Else
            Await UploadMedia(twitterCtx)
        End If

        Console.WriteLine()
        Console.Write("Press any key to continue...")
        Console.ReadKey()

    End Function

    Private Async Function SearchAsync(twitterCtx As TwitterContext) As Task
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

    Public Async Function UploadMedia(twitterCtx As TwitterContext) As Task
        Dim Clip_Filename As String = "..\..\..\Images\TwitterTest.mp4"
        Dim imageBytes As Byte() = File.ReadAllBytes(Clip_Filename)
        Const JoeMayoUserID As ULong = 15411837
        Dim additionalOwners = New ULong() {JoeMayoUserID}
        Dim status As String = "Testing video upload tweet #Linq2Twitter £ " +
                DateTime.Now.ToString(CultureInfo.InvariantCulture)
        Dim mediaType1 As String = "video/mp4"
        Dim mediaCategory As String = "tweet_video"
        Dim media As Media = Await twitterCtx.UploadMediaAsync(imageBytes, mediaType1, additionalOwners, mediaCategory)
        Dim mediaStatusResponse As Media = Nothing
        Do

            If mediaStatusResponse IsNot Nothing Then
                Dim checkAfterSeconds As Integer = If(mediaStatusResponse?.ProcessingInfo?.CheckAfterSeconds, 0)
                Console.WriteLine($"Twitter video testing in progress - waiting {checkAfterSeconds} seconds.")
                Await Task.Delay(checkAfterSeconds * 1000)
            End If
            mediaStatusResponse = Await (From stat In twitterCtx.Media Where stat.Type = MediaType.Status AndAlso stat.MediaID = media.MediaID Select stat).SingleOrDefaultAsync()


        Loop While mediaStatusResponse?.ProcessingInfo?.State = MediaProcessingInfo.InProgress

        If mediaStatusResponse?.ProcessingInfo?.State = MediaProcessingInfo.Succeeded Then
            Dim tweet22 As Status = Await twitterCtx.TweetAsync(status.Trim, New ULong() {media.MediaID})
            If tweet22 IsNot Nothing Then Console.WriteLine($"Tweet sent: {tweet22.Text}")
        Else
            Dim [error] As MediaError = mediaStatusResponse?.ProcessingInfo?.[Error]
            If [error] IsNot Nothing Then Console.WriteLine($"Request failed - Code: {[error].Code}, Name: {[error].Name}, Message: {[error].Message}")
        End If
    End Function
End Module
