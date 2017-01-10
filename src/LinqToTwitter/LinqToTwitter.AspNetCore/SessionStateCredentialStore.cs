using Microsoft.AspNetCore.Http;

namespace LinqToTwitter
{
    public class SessionStateCredentialStore : InMemoryCredentialStore
    {
        readonly ISession session;

        public SessionStateCredentialStore(ISession session)
        {
            this.session = session;
        }

        public override string ConsumerKey
        {
            get
            {
                return session.GetString("OAuthConsumerKey");
            }
            set
            {
                session.SetString("OAuthConsumerKey", value);
            }
        }

        public override string ConsumerSecret
        {
            get
            {
                return session.GetString("OAuthConsumerSecret");
            }
            set
            {
                session.SetString("OAuthConsumerSecret", value);
            }
        }

        public override string OAuthToken
        {
            get
            {
                return session.GetString("OAuthToken");
            }
            set
            {
                session.SetString("OAuthToken", value);
            }
        }

        public override string OAuthTokenSecret
        {
            get
            {
                return session.GetString("OAuthTokenSecret");
            }
            set
            {
                session.SetString("OAuthTokenSecret", value);
            }
        }

        public override string ScreenName
        {
            get
            {
                return session.GetString("ScreenNameToken");
            }
            set
            {
                session.SetString("ScreenNameToken", value);
            }
        }

        public override ulong UserID
        {
            get
            {
                string userIDString = session.GetString("UserIdToken") ?? "0";
                ulong.TryParse(userIDString, out ulong userID);
                return userID;
            }
            set
            {
                session.SetString("UserIdToken", value.ToString());
            }
        }
    }
}
