using System;
using System.Linq;

namespace LinqToTwitter.Security
{
    /// <summary>
    /// Implements RFC 2104
    /// </summary>
    public class Hmac : IHmac
    {
        const int Blocksize = 64;

        readonly IHash hash;

        public Hmac(IHash hash)
        {
            this.hash = hash;
        }

        public byte[] Sign(byte[] key, byte[] msg)
        {
            byte[] initializedKey = InitializeKey(key);

            byte[] oKeyPad = new byte[Blocksize];
            byte[] iKeyPad = new byte[Blocksize];

            for (int i = 0; i < Blocksize; i++)
            {
                oKeyPad[i] = (byte)(0x5c ^ initializedKey[i]);
                iKeyPad[i] = (byte)(0x36 ^ initializedKey[i]);
            }

            byte[] innerHash = hash.Compute(Concat(iKeyPad, msg));
            byte[] outerHash = hash.Compute(Concat(oKeyPad, innerHash));

            return outerHash;
        }

        byte[] InitializeKey(byte[] key)
        {
            byte[] initializedKey = null;

            if (key.Length > Blocksize)
            {
                byte[] hashedKey = hash.Compute(key);
                byte[] padding = Enumerable.Repeat<byte>(0x00, Blocksize - hashedKey.Length).ToArray();
                initializedKey = Concat(hashedKey, padding);
            }
            else if (key.Length < Blocksize)
            {
                byte[] padding = Enumerable.Repeat<byte>(0x00, Blocksize - key.Length).ToArray();
                initializedKey = Concat(key, padding);
            }
            else
            {
                initializedKey = key;
            }

            return initializedKey;
        }

        byte[] Concat(byte[] first, byte[] second)
        {
            byte[] combinedBytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combinedBytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, combinedBytes, first.Length, second.Length);
            return combinedBytes;
        }
    }
}
