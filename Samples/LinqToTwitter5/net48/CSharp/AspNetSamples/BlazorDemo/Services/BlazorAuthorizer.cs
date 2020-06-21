using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using LinqToTwitter;
using Microsoft.AspNetCore.Http;

namespace BlazorDemo.Services
{
    public class BlazorAuthorizer : AspNetAuthorizer
    {
        public HttpClient _httpClient;
        HttpContextAccessor _httpContextAccessor;
        private string _authUrl;

        public BlazorAuthorizer(
            HttpClient client, 
            HttpContextAccessor HttpContextAccessor
            )
        {
            _httpClient = client;
            _httpContextAccessor = HttpContextAccessor;
        }

        public string GetPathBase()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();
            return $@"{request.Scheme}://{host}{pathBase}/";
        }

        public async Task<ActionResult> Begin()
        {
            this.CredentialStore = new InMemoryCredentialStore();
            this.CredentialStore.ConsumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
            this.CredentialStore.ConsumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret");

            // to pass parameters that you can read in Complete(), via Request.QueryString, when Twitter returns
            // var parameters = new Dictionary<string, string> { { "my_custom_param", "val" } };
            // return await auth.BeginAuthorizationAsync(new Uri(GetPathBase()), parameters);

            return (RedirectResult)await BeginAuthorizationAsync(new Uri(GetPathBase()));
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