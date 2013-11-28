using System;
using System.Text;
using LinqToTwitter.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.SecurityTests
{
    /// <summary>
    /// Tests from RFC 2202
    /// </summary>
    [TestClass]
    public class HmacTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] expectedHash = { 0xb6, 0x17, 0x31, 0x86, 0x55, 0x05, 0x72, 0x64, 0xe2, 0x8b, 0xc0, 0xb6, 0xfb, 0x37, 0x8c, 0x8e, 0xf1, 0x46, 0xbe, 0x00 };
            byte[] key = { 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b };
            byte[] msg = Encoding.UTF8.GetBytes("Hi There");

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod2()
        {
            byte[] expectedHash = { 0xef, 0xfc, 0xdf, 0x6a, 0xe5, 0xeb, 0x2f, 0xa2, 0xd2, 0x74, 0x16, 0xd5, 0xf1, 0x84, 0xdf, 0x9c, 0x25, 0x9a, 0x7c, 0x79 };
            byte[] key = Encoding.UTF8.GetBytes("Jefe");
            byte[] msg = Encoding.UTF8.GetBytes("what do ya want for nothing?");

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod3()
        {
            const int Size = 50;
            byte[] expectedHash = { 0x12, 0x5d, 0x73, 0x42, 0xb9, 0xac, 0x11, 0xcd, 0x91, 0xa3, 0x9a, 0xf4, 0x8a, 0xa1, 0x7b, 0x4f, 0x63, 0xf1, 0x75, 0xd3 };
            byte[] key = { 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa };
            byte[] msg = new byte[Size];
            for (int i = 0; i < Size; i++) msg[i] = (byte)0xdd;

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod4()
        {
            const int Size = 50;
            byte[] expectedHash = { 0x4c, 0x90, 0x07, 0xf4, 0x02, 0x62, 0x50, 0xc6, 0xbc, 0x84, 0x14, 0xf9, 0xbf, 0x50, 0xc8, 0x6c, 0x2d, 0x72, 0x35, 0xda };
            byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19 };
            byte[] msg = new byte[Size];
            for (int i = 0; i < Size; i++) msg[i] = (byte)0xcd;

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod5()
        {
            byte[] expectedHash = { 0x4c, 0x1a, 0x03, 0x42, 0x4b, 0x55, 0xe0, 0x7f, 0xe7, 0xf2, 0x7b, 0xe1, 0xd5, 0x8b, 0xb9, 0x32, 0x4a, 0x9a, 0x5a, 0x04 };
            byte[] key = { 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c, 0x0c };
            byte[] msg = Encoding.UTF8.GetBytes("Test With Truncation");

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod6()
        {
            const int Size = 80;
            byte[] expectedHash = { 0xaa, 0x4a, 0xe5, 0xe1, 0x52, 0x72, 0xd0, 0x0e, 0x95, 0x70, 0x56, 0x37, 0xce, 0x8a, 0x3b, 0x55, 0xed, 0x40, 0x21, 0x12 };
            byte[] key = new byte[Size];
            for (int i = 0; i < Size; i++) key[i] = (byte)0xaa;
            byte[] msg = Encoding.UTF8.GetBytes("Test Using Larger Than Block-Size Key - Hash Key First");

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }

        [TestMethod]
        public void TestMethod7()
        {
            const int Size = 80;
            byte[] expectedHash = { 0xe8, 0xe9, 0x9d, 0x0f, 0x45, 0x23, 0x7d, 0x78, 0x6d, 0x6b, 0xba, 0xa7, 0x96, 0x5c, 0x78, 0x08, 0xbb, 0xff, 0x1a, 0x91 };
            byte[] key = new byte[Size];
            for (int i = 0; i < Size; i++) key[i] = (byte)0xaa;
            byte[] msg = Encoding.UTF8.GetBytes("Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data");

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            Assert.IsNotNull(hash);
            for (int i = 0; i < expectedHash.Length; i++)
                Assert.AreEqual(expectedHash[i], hash[i]);
        }
    }
}
