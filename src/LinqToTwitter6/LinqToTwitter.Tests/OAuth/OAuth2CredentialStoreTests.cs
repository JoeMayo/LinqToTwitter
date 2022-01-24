using LinqToTwitter.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.OAuth
{
    [TestClass]
    public class OAuth2CredentialStoreTests
    {
        OAuth2CredentialStore credStore;

        [TestInitialize]
        public void Init()
        {
            credStore = new OAuth2CredentialStore()
            {
                AccessToken = "123",
                ClientID = "456",
                ClientSecret = "789",
                CodeChallenge = "012",
                RedirectUri = "345",
                RefreshToken = "123",
                Scopes = new List<string>(),
                State = "678",
            };
        }

        [TestMethod]
        public async Task GenerateCodeChallenge_ReturnsValuesGreaterThanOrEqualTo44()
        {
            await credStore.ClearAsync();

            Assert.IsNull(credStore.AccessToken);
            Assert.IsNull(credStore.ClientID);
            Assert.IsNull(credStore.ClientSecret);
            Assert.IsNull(credStore.CodeChallenge);
            Assert.IsNull(credStore.RedirectUri);
            Assert.IsNull(credStore.RefreshToken);
            Assert.IsNull(credStore.Scopes);
            Assert.IsNull(credStore.State);
        }
    }
}
