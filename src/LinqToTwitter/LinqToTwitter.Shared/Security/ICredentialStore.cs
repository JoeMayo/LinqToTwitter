using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface ICredentialStore
    {
        /// <summary>
        /// Key provided by Twitter for your application
        /// </summary>
        string ConsumerKey { get; set; }

        /// <summary>
        /// Secret provided by Twitter for your application
        /// </summary>
        string ConsumerSecret { get; set; }

        /// <summary>
        /// Token provided by Twitter for making request
        /// </summary>
        string OAuthToken { get; set; }

        /// <summary>
        /// Unique access token for a user
        /// </summary>
        string OAuthTokenSecret { get; set; }

        /// <summary>
        /// Twitter screen name
        /// </summary>
        string ScreenName { get; set; }

        /// <summary>
        /// Twitter user ID
        /// </summary>
        ulong UserID { get; set; }

        /// <summary>
        /// Does this CredentialStore have all credentials populated?
        /// </summary>
        bool HasAllCredentials();

        /// <summary>
        /// Populates this with credential values from storage
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Saves credentials from this to storage
        /// </summary>
        Task StoreAsync();

        /// <summary>
        /// Removes credentials from storage
        /// </summary>
        Task ClearAsync();
    }
}
