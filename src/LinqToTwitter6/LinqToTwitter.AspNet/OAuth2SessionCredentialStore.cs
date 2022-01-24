using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    /// <summary>
    /// Credential Store for ASP.NET applications - Uses Session State
    /// </summary>
    public class OAuth2SessionCredentialStore : OAuth2CredentialStore
    {
        readonly ISession session;

        public OAuth2SessionCredentialStore(IHttpContextAccessor httpCtx)
        {
            session = httpCtx.HttpContext.Session;         
        }

        public OAuth2SessionCredentialStore(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Required for all clients - you can find this in the Twitter Developer portal
        /// </summary>
        public override string ClientID
        {
            get
            {
                return session.GetString("OAuth2ClientID");
            }
            set
            {
                session.SetString("OAuth2ClientID", value);
            }
        }

        /// <summary>
        /// Required for confidential clients - you can find this in the Twitter Developer portal
        /// </summary>
        public override string ClientSecret
        {
            get
            {
                return session.GetString("OAuth2ClientSecret");
            }
            set
            {
                session.SetString("OAuth2ClientSecret", value);
            }
        }

        /// <summary>
        /// Send when authorizing and getting access token to verify original source
        /// </summary>
        public override string CodeChallenge
        {
            get
            {
                return session.GetString("OAuth2CodeChallenge");
            }
            set
            {
                session.SetString("OAuth2CodeChallenge", value);
            }
        }

        /// <summary>
        /// Required - these are the permissions you want the user to give your app.
        /// See endpoint documentation for what scopes are required.
        /// </summary>
        public override IEnumerable<string> Scopes
        {
            get
            {
                return 
                    session
                        .GetString("OAuth2Scopes")
                        .Split(' ')
                        .ToList();
            }
            set
            {
                if (value != null)
                    session
                        .SetString(
                            "OAuth2Scopes", 
                            string.Join(' ', value));
            }
        }

        /// <summary>
        /// Populated after user approves your app and used for each command/query
        /// </summary>
        public override string AccessToken
        {
            get
            {
                return session.GetString("OAuth2AccessToken");
            }
            set
            {
                session.SetString("OAuth2AccessToken", value);
            }
        }

        /// <summary>
        /// Can be used to get a new AccessToken - only available if you authorized with `offline.access` scope.
        /// </summary>
        public override string RefreshToken
        {
            get
            {
                return session.GetString("OAuth2RefreshToken");
            }
            set
            {
                session.SetString("OAuth2RefreshToken", (string)value);
            }
        }

        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public override string RedirectUri
        {
            get
            {
                return session.GetString("OAuth2Callback");
            }
            set
            {
                session.SetString("OAuth2Callback", value);
            }
        }

        /// <summary>
        /// Value to verify against what was sent to Twitter and what was received.
        /// Helps prevent CSRF attack.
        /// </summary>
        public override string State
        {
            get
            {
                return session.GetString("OAuth2State");
            }
            set
            {
                session.SetString("OAuth2State", value);
            }
        }

        public override Task ClearAsync()
        {
            session.Remove("OAuth2ClientID");
            session.Remove("OAuth2ClientSecret");
            session.Remove("OAuth2CodeChallenge");
            session.Remove("OAuth2Scopes");
            session.Remove("OAuth2AccessToken");
            session.Remove("OAuth2RefreshToken");
            session.Remove("OAuth2Callback");
            session.Remove("OAuth2State");

            return Task.CompletedTask;
        }
    }
}
