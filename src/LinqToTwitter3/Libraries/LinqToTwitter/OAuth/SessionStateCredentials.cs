using System.Web;
using System.Web.SessionState;

namespace LinqToTwitter
{
    public class SessionStateCredentials : InMemoryCredentials
    {
        readonly HttpSessionState session;

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

        public override string ScreenName
        {
            get
            {
                return session["ScreenNameToken"] as string;
            }
            set
            {
                session["ScreenNameToken"] = value;
            }
        }

        public override string UserId
        {
            get
            {
                return session["UserIdToken"] as string;
            }
            set
            {
                session["UserIdToken"] = value;
            }
        }
    }
}
