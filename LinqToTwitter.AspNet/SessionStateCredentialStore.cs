using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace LinqToTwitter
{
    public class SessionStateCredentialStore : InMemoryCredentialStore
    {
        readonly HttpSessionState session;

        public SessionStateCredentialStore()
        {
            session = HttpContext.Current.Session;
        }

        public SessionStateCredentialStore(HttpSessionState session)
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

        public override string OAuthTokenSecret
        {
            get
            {
                return session["OAuthTokenSecret"] as string;
            }
            set
            {
                session["OAuthTokenSecret"] = value;
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

        public override ulong UserID
        {
            get
            {
                return (ulong)(session["UserIdToken"] ?? 0ul);
            }
            set
            {
                session["UserIdToken"] = value;
            }
        }
    }
}
