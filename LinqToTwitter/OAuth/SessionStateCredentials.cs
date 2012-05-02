using System.Web;
using System.Web.SessionState;

namespace LinqToTwitter
{
    public class SessionStateCredentials : InMemoryCredentials
    {
        private HttpSessionState session;

        public SessionStateCredentials()
        {
            session = HttpContext.Current.Session;
        }

        public SessionStateCredentials(HttpSessionState session)
        {
            this.session = session;
        }

        public override string ConsumerKey
        {
            get
            {
                return session["OAuthConsumerKey"] as string;
            }
            set
            {
                session["OAuthConsumerKey"] = value;
            }
        }

        public override string ConsumerSecret
        {
            get
            {
                return session["OAuthConsumerSecret"] as string;
            }
            set
            {
                session["OAuthConsumerSecret"] = value;
            }
        }

        public override string OAuthToken
        {
            get
            {
                return session["OAuthToken"] as string;
            }
            set
            {
                session["OAuthToken"] = value;
            }
        }

        public override string AccessToken
        {
            get
            {
                return session["OAuthAccessToken"] as string;
            }
            set
            {
                session["OAuthAccessToken"] = value;
            }
        }
    }
}
