
namespace LinqToTwitter.OAuth
{
    /// <summary>
    /// Used for XAuthAuthorization, which requires permission from Twitter before using
    /// </summary>
    public class XAuthCredentials : InMemoryCredentialStore
    {
        /// <summary>
        /// Twitter User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Twitter Password
        /// </summary>
        public string Password { get; set; }

        public override string ToString()
        {
            return base.ToString() + "," + UserName + "," + Password;
        }
    }
}
