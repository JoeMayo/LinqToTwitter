//Contributed by: Ayo Adesugba

using System;
using System.Linq;
using Windows.Storage;

namespace LinqToTwitter
{
    /// <summary>
    /// Synchronous persistence for WinRtAuthorizer
    /// </summary>
    public class WinRtSettingsCredentials : IOAuthCredentials, IWinRtSettingsCredentials
    {
        const int ConsumerKeyIdx = 0;
        const int ConsumerSecretIdx = 1;
        const int OAuthTokenIdx = 2;
        const int AccessTokenIdx = 3;
        const int ScreenNameIdx = 4;
        const int UserIdIdx = 5;

        readonly string[] credentials = new string[6];
        readonly ApplicationDataContainer settings;

        public WinRtSettingsCredentials(ApplicationDataContainer settings)
        {
            this.settings = settings;
            LoadCredentialsFromSettings();
        }

        public void Load(string credentialsString)
        {
            string[] tempCredentials = credentialsString.Split(',');

            for (int i = 0; i < tempCredentials.Length; i++)
            {
                credentials[i] = tempCredentials[i];
            }

            SaveCredentialsToSettings();
        }

        public override string ToString()
        {
            if (credentials == null)
            {
                LoadCredentialsFromSettings();
            }

            return string.Join(",", credentials);
        }

        public void Save()
        {
            SaveCredentialsToSettings();
        }

        public void Clear()
        {
            settings.DeleteContainer("TWStore");
        }

        public void LoadCredentialsFromSettings()
        {
            ApplicationDataContainer fbSettings = settings.CreateContainer("TWStore", ApplicationDataCreateDisposition.Always);

            if (settings.Containers.ContainsKey("TWStore"))
            {
                var credentialString = fbSettings.Values["credentials"];
                if (credentialString == null) return;
                if (!string.IsNullOrWhiteSpace(credentialString.ToString()))
                {
                    string[] tempCredentialArr = credentialString.ToString().Split(',');

                    for (int i = 0; i < tempCredentialArr.Length; i++)
                    {
                        credentials[i] = tempCredentialArr[i];
                    }
                }
            }
        }

        public void SaveCredentialsToSettings()
        {
            ApplicationDataContainer fbSettings = settings.CreateContainer("TWStore", ApplicationDataCreateDisposition.Always);
            var credentialsString = string.Join(",", credentials);

            if (!string.IsNullOrWhiteSpace(credentialsString))
            {
                if (settings.Containers.ContainsKey("TWStore"))
                    fbSettings.Values["credentials"] = credentialsString;
            }
        }

        public string AccessToken
        {
            get
            {
                if (credentials[AccessTokenIdx] == null)
                {
                    LoadCredentialsFromSettings();
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
                    LoadCredentialsFromSettings();
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
                    LoadCredentialsFromSettings();
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
                    LoadCredentialsFromSettings();
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
                    LoadCredentialsFromSettings();
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
                    LoadCredentialsFromSettings();
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
