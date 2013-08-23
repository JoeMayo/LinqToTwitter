using System;
using System.Linq;

namespace LinqToTwitter.Security
{
    public interface IHmac
    {
        byte[] Sign(byte[] key, byte[] msg);
    }
}