using System;
using System.Text;
using LinqToTwitter.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.SecurityTests
{
    /// <summary>
    /// Tests from RFC 3174 + more
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

        [TestMethod]
        public void LettarAHashesCorrectly()
        {
            byte[] expectedResults = { 0x86, 0xF7, 0xE4, 0x37, 0xFA, 0xA5, 0xA7, 0xFC, 0xE1, 0x5D, 0x1D, 0xDC, 0xB9, 0xEA, 0xEA, 0xEA, 0x37, 0x76, 0x67, 0xB8 };

            byte[] hashBytes = new Sha1().Compute(Encoding.UTF8.GetBytes("a"));

            Assert.IsNotNull(hashBytes);
            for (int i = 0; i < expectedResults.Length; i++)
                Assert.AreEqual(expectedResults[i], hashBytes[i]);
        }

        [TestMethod]
        public void OctalSequenceHashesCorrectly()
        {
            byte[] expectedResults = { 0xE0, 0xC0, 0x94, 0xE8, 0x67, 0xEF, 0x46, 0xC3, 0x50, 0xEF, 0x54, 0xA7, 0xF5, 0x9D, 0xD6, 0x0B, 0xED, 0x92, 0xAE, 0x83 };

            byte[] hashBytes = new Sha1().Compute(Encoding.UTF8.GetBytes("0123456701234567012345670123456701234567012345670123456701234567"));

            Assert.IsNotNull(hashBytes);
            for (int i = 0; i < expectedResults.Length; i++)
                Assert.AreEqual(expectedResults[i], hashBytes[i]);
        }
    }
}
