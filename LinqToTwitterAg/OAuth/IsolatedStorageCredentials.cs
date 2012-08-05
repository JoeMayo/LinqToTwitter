using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace LinqToTwitter
{
    public class IsolatedStorageCredentials : IOAuthCredentials
    {
        const int ConsumerKeyIdx = 0;
        const int ConsumerSecretIdx = 1;
        const int OAuthTokenIdx = 2;
        const int AccessTokenIdx = 3;

        readonly string[] credentials = new string[4];

        public IsolatedStorageCredentials()
        {
            LoadCredentialsFromIsolatedStorage();
        }

        public void Load(string credentialsString)
        {
            string[] tempCredentials = credentialsString.Split(',');

            for (int i = 0; i < tempCredentials.Length; i++)
            {
                credentials[i] = tempCredentials[i];
            }

            SaveCredentialsToIsolatedStorage();
        }

        public override string ToString()
        {
            if (credentials == null)
            {
                LoadCredentialsFromIsolatedStorage();
            }

            return string.Join(",", credentials);
        }

        public void Save()
        {
            SaveCredentialsToIsolatedStorage();
        }

        public void Clear()
        {
            for (int i = 0; i < credentials.Length; i++)
            {
                credentials[i] = string.Empty;
            }

            SaveCredentialsToIsolatedStorage();
        }

        void LoadCredentialsFromIsolatedStorage()
        {
            string tempCredentialsString = null;

            IsolatedStorageFile credentialsStore = IsolatedStorageFile.GetUserStoreForApplication();

            using (var isoFileStream = new IsolatedStorageFileStream("Linq2TwitterCredentials.txt", FileMode.OpenOrCreate, credentialsStore))
            {
                using (var isoFileReader = new StreamReader(isoFileStream))
                {
                    tempCredentialsString = isoFileReader.ReadLine();
                }
            }

            if (tempCredentialsString != null)
            {
                string[] tempCredentialsArr = tempCredentialsString.Split(',');

                for (int i = 0; i < tempCredentialsArr.Length; i++)
                {
                    credentials[i] = tempCredentialsArr[i];
                }
            }
        }

        void SaveCredentialsToIsolatedStorage()
        {
            var credentialsString = string.Join(",", credentials);

            IsolatedStorageFile credentialsStore = IsolatedStorageFile.GetUserStoreForApplication();

            using (var isoFileStream = new IsolatedStorageFileStream("Linq2TwitterCredentials.txt", FileMode.OpenOrCreate, credentialsStore))
            {
                using (var isoFileWriter = new StreamWriter(isoFileStream))
                {
                    isoFileWriter.WriteLine(credentialsString);
                }
            }
        }

        public string AccessToken
        {
            get
            {
                if (credentials[AccessTokenIdx] == null)
                {
                    LoadCredentialsFromIsolatedStorage();
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
                    LoadCredentialsFromIsolatedStorage();
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
                    LoadCredentialsFromIsolatedStorage();
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
                    LoadCredentialsFromIsolatedStorage();
                }

                return credentials[ConsumerKeyIdx];
            }
            set
            {
                credentials[ConsumerKeyIdx] = value;
            }
        }
    }
}
