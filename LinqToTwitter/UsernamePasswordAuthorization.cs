//-----------------------------------------------------------------------
// <copyright file="Utilities.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace LinqToTwitter
{
    /// <summary>
    /// Standard HTTP Basic authentication that takes a Twitter username and password
    /// for authenticating Twitter client applications to access private user data.
    /// </summary>
    [Serializable]
    public class UsernamePasswordAuthorization : ITwitterAuthorization
    {
        /// <summary>
        /// The owner window of any popup UI this authorization module presents to the user.
        /// </summary>
        private IWin32Window ownerWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordAuthorization"/> class.
        /// </summary>
        public UsernamePasswordAuthorization()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordAuthorization"/> class.
        /// </summary>
        /// <param name="serviceUri">The base URL of the Twitter service being authenticated to.  Must not be null.</param>
        /// <param name="ownerWindow">The parent window of any UI that may be presented to the user as part of authentication.</param>
        public UsernamePasswordAuthorization(IWin32Window ownerWindow)
        {
            this.ownerWindow = ownerWindow;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        #region ITwitterAuthorization Members

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Indicates if you want to use the proxy
        /// </summary>
        public bool UseProxy { get; set; }

        /// <summary>
        /// Gets or sets the proxy address
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Gets or sets the proxy port
        /// </summary>
        public int ProxyPort { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the read write timeout.
        /// </summary>
        /// <value>The read write timeout.</value>
        public TimeSpan ReadWriteTimeout { get; set; }

        /// <summary>
        /// Gets the UserID that Twitter has assigned to the logged in user.
        /// </summary>
        /// <value>An integer number, represented as a string.</value>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the screenname of the user logged into Twitter.
        /// </summary>
        public string ScreenName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is ready to send authorized GET and POST requests.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthorized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this authorization mechanism can immediately
        /// provide the user with access to his account without prompting (again)
        /// for his credentials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
        /// </value>
        public bool CachedCredentialsAvailable
        {
            get
            {
                return Kerr.Credential.Exists(this.AuthenticationTarget, Kerr.CredentialType.Generic);
            }
        }

        /// <summary>
        /// Gets or sets the base Service URI of the Twitter service to authenticate to.
        /// </summary>
        public string AuthenticationTarget { get; set; }

        /// <summary>
        /// Clears the cached credentials, if any.
        /// </summary>
        public void ClearCachedCredentials()
        {
            Kerr.Credential.Delete(this.AuthenticationTarget, Kerr.CredentialType.Generic);
        }

        /// <summary>
        /// Logs the user into the web site, prompting for credentials if necessary.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the authentication/authorization.</exception>
        public void SignOn()
        {
            if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password))
            {
                try
                {
                    ValidateCredentials(this.UserName, this.Password);
                    return;
                }
                catch (WebException)
                {
                    // Something about the credentials is not right.
                    this.Password = null;
                }
            }

            if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password))
            {
                using (Kerr.PromptForCredential prompt = new Kerr.PromptForCredential())
                {
                    prompt.TargetName = this.AuthenticationTarget;
                    prompt.Title = "Authorize Twitter client";
                    prompt.Message = "Enter your Twitter network credentials to authorize this application to read and post your updates.";
                    prompt.UserName = this.UserName ?? string.Empty;
                    prompt.ExpectConfirmation = true;
                    //prompt.Persist = false;
                    prompt.GenericCredentials = true;

                    if (prompt.ShowDialog(this.ownerWindow) == DialogResult.OK)
                    {
                        try
                        {
                            ValidateCredentials(prompt.UserName, prompt.Password.ToUnsecureString());
                            this.UserName = prompt.UserName;
                            this.Password = prompt.Password.ToUnsecureString();
                            if (prompt.SaveChecked)
                            {
                                prompt.ConfirmCredentials();
                            }
                        }
                        catch (WebException)
                        {
                            // Make sure that if these were cached credentials that we clear them.
                            this.ClearCachedCredentials();
                            throw; // TODO: wrap exception appropriately.
                        }
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }
            }
        }

        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        private void ValidateCredentials(string userName, string password)
        {
            try
            {
                this.UserName = userName;
                this.Password = password;
                this.IsAuthorized = true;

                var request = Get(new Uri("https://twitter.com/account/verify_credentials.xml"), null);

                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var xdoc = XDocument.Load(XmlReader.Create(responseStream));
                        this.UserId = xdoc.Element("user").Element("id").Value;
                        this.ScreenName = xdoc.Element("user").Element("screen_name").Value;
                    }
                }
            }
            catch
            {
                this.IsAuthorized = false;
                throw;
            }
        }

        /// <summary>
        /// Where applicable, cancels session tokens (like an HTTP cookie), effectively logging the user off.
        /// </summary>
        public void SignOff()
        {
            this.ScreenName = null;
            this.UserId = null;
            this.IsAuthorized = false;
        }

        /// <summary>
        /// Prepares an authorized HTTP GET request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The arguments to include in the query string.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        public HttpWebRequest Get(Uri requestUri, IDictionary<string, string> args)
        {
            Uri requestUriWithArgs = Utilities.AppendQueryString(requestUri, args);
            var req = (HttpWebRequest)WebRequest.Create(requestUriWithArgs);
            this.InitializeRequest(req);
            return req;
        }

        /// <summary>
        /// Prepares an authorized HTTP POST request without sending a POST entity stream.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns>The <see cref="HttpWebRequest"/> object that may be further customized.</returns>
        public HttpWebRequest Post(Uri requestUri)
        {
            var req = (HttpWebRequest)WebRequest.Create(requestUri);
            this.InitializeRequest(req);
            req.Method = "POST";
            return req;
        }

        /// <summary>
        /// Prepares and sends an authorized HTTP POST request.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="args">The parameters to include in the POST entity.  Must not be null.</param>
        /// <returns>
        /// The HTTP response.
        /// </returns>
        /// <exception cref="WebException">Thrown if the server returns an error.</exception>
        public HttpWebResponse Post(Uri requestUri, IDictionary<string, string> args)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            string queryString = Utilities.BuildQueryString(args);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);

            var req = (HttpWebRequest)WebRequest.Create(requestUri);
            this.InitializeRequest(req);
            req.Method = "POST";
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "x-www-form-urlencoded";
            req.ContentLength = queryStringBytes.Length;

            using (Stream requestStream = req.GetRequestStream())
            {
                requestStream.Write(queryStringBytes, 0, queryStringBytes.Length);
            }

            return (HttpWebResponse)req.GetResponse();
        }

        #endregion

        /// <summary>
        /// Initializes the request in ways common to GET and POST requests.
        /// </summary>
        /// <param name="request">The request to initialize.</param>
        private void InitializeRequest(HttpWebRequest request)
        {
            if (this.IsAuthorized)
            {
                request.Credentials = new NetworkCredential(this.UserName, this.Password);

                // From issue #25135 (icyflash on codeplex.com):
                // When using a twitter api proxy (such as http://code.google.com/p/birdnest/), you will receive an HTTP Basic Authentication response 401.
                // Fix: 
                //add HTTP Basic Authentication to http request header
                string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(this.UserName + ":" + this.Password));
                request.Headers["Authorization"] = "Basic " + authInfo;
            }

            request.UserAgent = this.UserAgent;

            // From issue #24811 (MarkStruik on codeplex.com):
            //      If you are behind a proxy server you cant use the application. 
            // Fix includes UseProxy, ProxyAddress, and ProxyPort properties
            if (UseProxy)
            {
                request.Proxy = new WebProxy(ProxyAddress, ProxyPort);
            }

            if (this.ReadWriteTimeout > TimeSpan.Zero)
            {
                request.ReadWriteTimeout = (int)this.ReadWriteTimeout.TotalMilliseconds;
            }

            if (this.Timeout > TimeSpan.Zero)
            {
                request.Timeout = (int)this.Timeout.TotalMilliseconds;
            }
        }
    }
}
