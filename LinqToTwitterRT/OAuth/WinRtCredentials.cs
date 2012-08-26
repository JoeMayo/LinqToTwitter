/******************************************
 * Contributor: Sumit Maitra - Twitter @sumitkm
 * Date: 8/21/2012
 ******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LinqToTwitter
{
    public class WinRtCredentials : IOAuthCredentials
    {
        const int ConsumerKeyIdx = 0;
        const int ConsumerSecretIdx = 1;
        const int OAuthTokenIdx = 2;
        const int AccessTokenIdx = 3;
        const int ScreenNameIdx = 4;
        const int UserIdIdx = 5;

        readonly string[] credentials = new string[6];
        readonly StorageFolder folder;

        public WinRtCredentials(StorageFolder folder)
        {
            this.folder = folder;
            LoadCredentialsFromStorageFile();
        }

        public void Load(string credentialsString)
        {
            string[] tempCredentials = credentialsString.Split(',');

            for (int i = 0; i < tempCredentials.Length; i++)
            {
                credentials[i] = tempCredentials[i];
            }

            SaveCredentialsToStorageFileAsync();
        }

        public override string ToString()
        {
            if (credentials == null)
            {
                LoadCredentialsFromStorageFile();
            }

            return string.Join(",", credentials);
        }

        public void Save()
        {
            SaveCredentialsToStorageFileAsync();
        }

        public void Clear()
        {
            for (int i = 0; i < credentials.Length; i++)
            {
                credentials[i] = string.Empty;
            }

            SaveCredentialsToStorageFileAsync();
        }

        void LoadCredentialsFromStorageFile()
        {
            Task<StorageFile> credentialsFileTask =
                folder.CreateFileAsync(
                    "Linq2TwitterCredentials.txt",
                    CreationCollisionOption.OpenIfExists)
                    .AsTask();
            credentialsFileTask.Wait();

            Task<string> credentialsStringTask = 
                 FileIO.ReadTextAsync(credentialsFileTask.Result)
                 .AsTask();
            credentialsStringTask.Wait();

            string credentialsString = credentialsStringTask.Result;
            if (!string.IsNullOrWhiteSpace(credentialsString))
            {
                string[] tempCredentialsArr = credentialsString.Split(',');

                for (int i = 0; i < tempCredentialsArr.Length; i++)
                {
                    credentials[i] = tempCredentialsArr[i];
                }
            }
        }

        async void SaveCredentialsToStorageFileAsync()
        {
            var credentialsString = string.Join(",", credentials);

            if (!string.IsNullOrWhiteSpace(credentialsString))
            {
                StorageFile sampleFile =
                    await folder.CreateFileAsync(
                        "Linq2TwitterCredentials.txt",
                        CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(sampleFile, credentialsString); 
            }
        }

        public string AccessToken
        {
            get
            {
                if (credentials[AccessTokenIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }

                return credentials[AccessTokenIdx];
            }
            set
            {
                credentials[AccessTokenIdx] = value;
            }
        }

        public string ConsumerSecret
        {
            get
            {
                if (credentials[ConsumerSecretIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }

                return credentials[ConsumerSecretIdx];
            }
            set
            {
                credentials[ConsumerSecretIdx] = value;
            }
        }

        public string OAuthToken
        {
            get
            {
                if (credentials[OAuthTokenIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }

                return credentials[OAuthTokenIdx];
            }
            set
            {
                credentials[OAuthTokenIdx] = value;
            }
        }

        public string ConsumerKey
        {
            get
            {
                if (credentials[ConsumerKeyIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }

                return credentials[ConsumerKeyIdx];
            }
            set
            {
                credentials[ConsumerKeyIdx] = value;
            }
        }

        public string ScreenName
        {
            get
            {
                if (credentials[ScreenNameIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }
                return credentials[ScreenNameIdx];
            }
            set
            {
                credentials[ScreenNameIdx] = value;
            }
        }

        public string UserId
        {
            get
            {
                if (credentials[UserIdIdx] == null)
                {
                    LoadCredentialsFromStorageFile();
                }
                return credentials[UserIdIdx];
            }
            set
            {
                credentials[UserIdIdx] = value;
            }
        }
    }
}
