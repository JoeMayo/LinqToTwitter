//-----------------------------------------------------------------------
// <copyright file="UsernamePasswordAuthorization.cs">
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
    /// This module uses the Windows Credential store to securely store and retrieve credentials.
    /// </summary>
    /// <remarks>
    /// Due to its dependency on a 32-bit native DLL, this class only works on 32-bit machines
    /// with full trust.  Use <see cref="UsernamePasswordSimpleAuthorization"/> instead if this
    /// is a problem.
    /// </remarks>
    [Serializable]
    public class UsernamePasswordAuthorization : UsernamePasswordSimpleAuthorization
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
            this.AllowUIPrompt = true;
            this.ownerWindow = ownerWindow;
        }

        /// <summary>
        /// Gets a value indicating whether this authorization mechanism can immediately
        /// provide the user with access to his account without prompting (again)
        /// for his credentials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if cached credentials are available; otherwise, <c>false</c>.
        /// </value>
        public override bool CachedCredentialsAvailable
        {
            get
            {
                return Kerr.Credential.Exists(this.AuthenticationTarget, Kerr.CredentialType.Generic);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a dialog can appear to prompt the user for credentials.
        /// </summary>
        /// <value><c>true</c> for most desktop apps; for web apps or custom desktop apps, use <c>false</c>.</value>
        public bool AllowUIPrompt { get; set; }

        /// <summary>
        /// Clears the cached credentials, if any.
        /// </summary>
        public override void ClearCachedCredentials()
        {
            Kerr.Credential.Delete(this.AuthenticationTarget, Kerr.CredentialType.Generic);
        }

        /// <summary>
        /// Logs the user into the web site, prompting for credentials if necessary if <see cref="AllowUIPrompt"/>
        /// is set to <c>true</c>.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the user cancels the authentication/authorization.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the credentials are missing or invalid.</exception>
        public override void SignOn()
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

            if (this.AllowUIPrompt)
            {
                PromptForCredentials();
            }
            else
            {
                // TODO: what's a more appropriate exception type here?
                throw new InvalidOperationException("Invalid or missing Twitter credentials.");
            }
        }

        /// <summary>
        /// Prompts the user for credentials using a Windows dialog.
        /// </summary>
        private void PromptForCredentials()
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
}
