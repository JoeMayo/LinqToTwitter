using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using LinqToTwitter;

namespace BlazorDemo.Services
{
    public class BlazorAuthorizer : AspNetAuthorizer
    {
        public HttpClient _httpClient;
        private string _authUrl;

        public BlazorAuthorizer(HttpClient client)
        {
            _httpClient = client;
        }  

        public async Task<ActionResult> BeginAuthorizationAsync(
            Uri callback, Dictionary<string, string> parameters = null)
        {
            if (GoToTwitterAuthorization == null)
                GoToTwitterAuthorization = authUrl => { _authUrl = authUrl; };

            Callback = callback;

            await base.BeginAuthorizeAsync(callback, parameters).ConfigureAwait(false);

            return new RedirectResult(_authUrl,false);
        }
    }
}