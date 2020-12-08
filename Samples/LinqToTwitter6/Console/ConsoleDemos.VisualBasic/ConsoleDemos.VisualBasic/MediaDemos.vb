Imports System.Globalization
Imports System.IO
Imports LinqToTwitter

Public Class MediaDemos
    Public Shared Async Function RunAsync(twitterCtx As TwitterContext) As Task
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
            mediaStatusResponse = Await(From stat In twitterCtx.Media Where stat.Type = MediaType.Status AndAlso stat.MediaID = media.MediaID Select stat).SingleOrDefaultAsync()


        Loop While mediaStatusResponse?.ProcessingInfo?.State = MediaProcessingInfo.InProgress

        If mediaStatusResponse?.ProcessingInfo?.State = MediaProcessingInfo.Succeeded Then
            Dim tweet22 As Status = Await twitterCtx.TweetAsync(status.Trim, New ULong() {media.MediaID})
            If tweet22 IsNot Nothing Then Console.WriteLine($"Tweet sent: {tweet22.Text}")
        Else
            Dim [error] As MediaError = mediaStatusResponse?.ProcessingInfo?.[Error]
            If [error] IsNot Nothing Then Console.WriteLine($"Request failed - Code: {[error].Code}, Name: {[error].Name}, Message: {[error].Message}")
        End If
    End Function
End Class
