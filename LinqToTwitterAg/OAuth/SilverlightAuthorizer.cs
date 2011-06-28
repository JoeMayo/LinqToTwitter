using System;

namespace LinqToTwitter
{
    public class SilverlightAuthorizer : OAuthAuthorizer, ITwitterAuthorizer
    {
        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public Uri Callback { get; set; }

        /// <summary>
        /// This is a hook where you can assign
        /// a lambda to perform the technology
        /// specific redirection action.
        /// 
        /// The string passed as the lambda paramter
        /// is the Twitter authorization URL.
        /// </summary>
        public Action<string> PerformRedirect { get; set; }

        /// <summary>
        /// Perform authorization
        /// </summary>
        public void Authorize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously starts the authorization process
        /// </summary>
        /// <param name="authorizationCompleteCallback">Action you provide for when authorization completes.</param>
        public void BeginAuthorize(Uri callback, Action<TwitterAsyncResponse<object>> authorizationCompleteCallback)
        {
            if (IsAuthorized) return;

            if (PerformRedirect == null)
            {
                throw new InvalidOperationException("GoToTwitterAuthorization must have a handler before calling BeginAuthorize.");
            }

            OAuthTwitter.GetRequestTokenAsync(new Uri(OAuthRequestTokenUrl), new Uri(OAuthAuthorizeUrl), callback.ToString(), false, PerformRedirect, authorizationCompleteCallback);
        }

        public bool IsAuthorizing 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Credentials.OAuthToken);
            }
        }

        /// <summary>
        /// Asynchronously finishes the authorization process
        /// </summary>
        /// <param name="callback">Callback Url that Twitter has added parameters to</param>
        /// <param name="authorizationCompleteCallback">Action you provide for when authorization completes.</param>
        public void CompleteAuthorize(Uri callback, Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback)
        {
            if (IsAuthorized) return;

            const int QueryPart = 1;
            string[] callbackParts = callback.OriginalString.Split('?');

            if (callbackParts.Length == 2)
            {
                string oauthToken = OAuthTwitter.GetUrlParamValue(callbackParts[QueryPart], "oauth_token");
                Credentials.OAuthToken = oauthToken;
                OAuthTwitter.OAuthToken = oauthToken;

                // TODO: Page navigation (i.e. Twitter # handling) is a little quirky, this needs more analysis - Joe

                // we have to split on # because Twitter appends #PageName at the end of the url, 
                // which identifies the Silverlight page to navigate to, but is not part of the verifier
                string verifier = 
                    OAuthTwitter
                        .GetUrlParamValue(callbackParts[QueryPart], "oauth_verifier")
                        .Split('#')[0];

                if (verifier != null)
                {
                    OAuthTwitter.GetAccessTokenAsync(verifier, new Uri(OAuthAccessTokenUrl), null, authorizationCompleteCallback);
                } 
            }
        }
    }
}
