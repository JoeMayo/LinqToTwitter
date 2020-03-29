using System;
using System.Linq;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.Common
{
    [TestClass]
    public class EntitiesTests
    {
        [TestMethod]
        public void InstantiateEntities_WithNull_SuccessfullyCreatesEmptyCollectionProperties()
        {
            var emptyEntities = new Entities(null);

            Assert.IsNotNull(emptyEntities);
            Assert.IsNotNull(emptyEntities.HashTagEntities);
            Assert.IsFalse(emptyEntities.HashTagEntities.Any());
            Assert.IsNotNull(emptyEntities.MediaEntities);
            Assert.IsFalse(emptyEntities.MediaEntities.Any());
            Assert.IsNotNull(emptyEntities.SymbolEntities);
            Assert.IsFalse(emptyEntities.SymbolEntities.Any());
            Assert.IsNotNull(emptyEntities.UrlEntities);
            Assert.IsFalse(emptyEntities.UrlEntities.Any());
            Assert.IsNotNull(emptyEntities.UserMentionEntities);
            Assert.IsFalse(emptyEntities.UserMentionEntities.Any());
        }
    }
}
