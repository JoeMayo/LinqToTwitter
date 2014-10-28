using System;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IAsyncOAuthCredentials : IOAuthCredentials
    {
        Task ClearAsync();

        Task LoadCredentialsFromStorageFileAsync();

        Task SaveCredentialsToStorageFileAsync();
    }
}