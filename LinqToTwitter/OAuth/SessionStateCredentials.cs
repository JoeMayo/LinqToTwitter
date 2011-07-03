using System.Web;
using System.Web.SessionState;

namespace LinqToTwitter
{
    public class SessionStateCredentials : InMemoryCredentials
    {
        private HttpSessionState m_session;

        public SessionStateCredentials()
        {
            m_session = HttpContext.Current.Session;
        }

        public SessionStateCredentials(HttpSessionState session)
        {
            m_session = session;
        }

        public override string ConsumerKey
        {
            get
            {
                return m_session["OAuthConsumerKey"] as string;
            }
            set
            {
                m_session["OAuthConsumerKey"] = value;
            }
        }

        public override string ConsumerSecret
        {
            get
            {
                return m_session["OAuthConsumerSecret"] as string;
            }
            set
            {
                m_session["OAuthConsumerSecret"] = value;
            }
        }

        public override string OAuthToken
        {
            get
            {
                return m_session["OAuthToken"] as string;
            }
            set
            {
                m_session["OAuthToken"] = value;
            }
        }

        public override string AccessToken
        {
            get
            {
                return m_session["OAuthAccessToken"] as string;
            }
            set
            {
                m_session["OAuthAccessToken"] = value;
            }
        }
    }
}
