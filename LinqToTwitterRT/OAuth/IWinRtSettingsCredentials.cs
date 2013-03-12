namespace LinqToTwitter
{
    public interface IWinRtSettingsCredentials : IOAuthCredentials
    {
        void Clear();

        void LoadCredentialsFromSettings();

        void SaveCredentialsToSettings();
    }
}