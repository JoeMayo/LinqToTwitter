using System;
using System.Text;
using LinqToTwitter.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.SecurityTests
{
    /// <summary>
    /// Tests from RFC 3174
    /// </summary>
    [TestClass]
    public class Sha1Tests
    {
        [TestMethod]
        public void AbcHashesCorrectly()
        {
            byte[] expectedResults = { 0xA9, 0x99, 0x3E, 0x36, 0x47, 0x06, 0x81, 0x6A, 0xBA, 0x3E, 0x25, 0x71, 0x78, 0x50, 0xC2, 0x6C, 0x9C, 0xD0, 0xD8, 0x9D };

            byte[] hashBytes = new Sha1().Compute(Encoding.UTF8.GetBytes("abc"));

            Assert.IsNotNull(hashBytes);
            for (int i = 0; i < expectedResults.Length; i++)
                Assert.AreEqual(expectedResults[i], hashBytes[i]);
        }

        [TestMethod]
        public void AlternatingLettersHashesCorrectly()
        {
            byte[] expectedResults = { 0x84, 0x98, 0x3E, 0x44, 0x1C, 0x3B, 0xD2, 0x6E, 0xBA, 0xAE, 0x4A, 0xA1, 0xF9, 0x51, 0x29, 0xE5, 0xE5, 0x46, 0x70, 0xF1 };

            byte[] hashBytes = new Sha1().Compute(Encoding.UTF8.GetBytes("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"));

            Assert.IsNotNull(hashBytes);
            for (int i = 0; i < expectedResults.Length; i++)
                Assert.AreEqual(expectedResults[i], hashBytes[i]);
        }
    }
}
