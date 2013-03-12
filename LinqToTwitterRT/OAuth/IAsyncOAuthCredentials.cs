using System;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IAsyncOAuthCredentials : IOAuthCredentials
    {
        Task Clear();

        Task LoadCredentialsFromStorageFileAsync();

        Task SaveCredentialsToStorageFileAsync();
    }
}