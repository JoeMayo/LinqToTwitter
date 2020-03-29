using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace LinqToTwitter
{
    public class WinRtCredentials : InMemoryCredentials, IAsyncOAuthCredentials
    {
        const int ConsumerKeyIdx = 0;
        const int ConsumerSecretIdx = 1;
        const int OAuthTokenIdx = 2;
        const int AccessTokenIdx = 3;
        const int ScreenNameIdx = 4;
        const int UserIdIdx = 5;

        readonly StorageFolder folder;

        public string FileName { get; set; }

        public WinRtCredentials(StorageFolder folder, string fileName)
        {
            this.folder = folder;
            FileName = fileName ?? "Linq2TwitterCredentials.txt";
        }

        public async Task ClearAsync()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            if (files.Any(storFile => storFile.Name == FileName))
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
                await file.DeleteAsync();
            }
        }

        public async Task LoadCredentialsFromStorageFileAsync()
        {
            var credentialsFile =
                await folder.CreateFileAsync(
                    FileName,
                    CreationCollisionOption.OpenIfExists);

            var credentialsString = await
                 FileIO.ReadTextAsync(credentialsFile);
            
            if (!string.IsNullOrWhiteSpace(credentialsString))
            {
                string[] tempCredentialsArr = credentialsString.Split(',');

                ConsumerKey = tempCredentialsArr[ConsumerKeyIdx];
                ConsumerSecret = tempCredentialsArr[ConsumerSecretIdx];
                OAuthToken = tempCredentialsArr[OAuthTokenIdx];
                AccessToken = tempCredentialsArr[AccessTokenIdx];
                ScreenName = tempCredentialsArr[ScreenNameIdx];
                UserId = tempCredentialsArr[UserIdIdx];
            }
        }

        public async Task SaveCredentialsToStorageFileAsync()
        {
            var credentialsString =
                string.Join(",",
                    new List<string>
                    {
                        ConsumerKey, ConsumerSecret,
                        OAuthToken, AccessToken,
                        ScreenName, UserId
                    });

            if (!string.IsNullOrWhiteSpace(credentialsString))
            {
                StorageFile sampleFile =
                    await folder.CreateFileAsync(
                        FileName,
                        CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(sampleFile, credentialsString); 
            }
        }
    }
}
