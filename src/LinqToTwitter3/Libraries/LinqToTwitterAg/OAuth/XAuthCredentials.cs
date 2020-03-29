
namespace LinqToTwitter
{
    /// <summary>
    /// Used for XAuthAuthorization, which requires permission from Twitter before using
    /// </summary>
    public class XAuthCredentials : InMemoryCredentials
    {
        /// <summary>
        /// Twitter User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Twitter Password
        /// </summary>
        public string Password { get; set; }

        public override void Load(string credentialString)
        {
            base.Load(credentialString);

            string[] credentials = credentialString.Split(',');

            UserName = credentials[4];
            Password = credentials[5];
        }

        public override string ToString()
        {
            return base.ToString() + "," + UserName + "," + Password;
        }
    }
}
