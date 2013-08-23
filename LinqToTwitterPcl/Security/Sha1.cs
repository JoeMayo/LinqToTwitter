using System;
using System.Linq;

namespace LinqToTwitter.Security
{
    /// <summary>
    /// Implements RFC 3174
    /// </summary>
    public class Sha1 : IHash
    {
        const int HashSize = 20;

        const uint K0 = 0x5A827999;
        const uint K1 = 0x6ED9EBA1;
        const uint K2 = 0x8F1BBCDC;
        const uint K3 = 0xCA62C1D6;

        class Context
        {
            public uint A, B, C, D, E;

            public int MessageBlockIndex;
            public readonly byte[] MessageBlock = new byte[64];

            public readonly uint[] IntermediateHash = 
            {
                0x67452301,
                0xEFCDAB89,
                0x98BADCFE,
                0x10325476,
                0xC3D2E1F0
            };

            public uint LengthHigh;
            public uint LengthLow;
        }

        uint CircularShift(int bits, uint word)
        {
            return ((word << bits) | (word >> (32 - bits)));
        }

        public byte[] Compute(byte[] message)
        {
            var ctx = new Context();

            foreach(var msgByte in message)
            {
                ctx.MessageBlock[ctx.MessageBlockIndex++] = (byte)(msgByte & 0xFF);

                ctx.LengthLow += 8;
                if (ctx.LengthLow == 0)
                    ctx.LengthHigh++;

                if (ctx.MessageBlockIndex == 64)
                    ProcessMessageBlock(ctx);
            }

            PadMessage(ctx);

            var msgDigest = new byte[HashSize];

            for (int i = 0; i < HashSize; i++)
                msgDigest[i] = (byte)(ctx.IntermediateHash[i >> 2] >> 8 * (3 - (i & 0x03)));

            return msgDigest;
        }
  
        void ProcessMessageBlock(Context ctx)
        {
            uint[] w = new uint[80];

            for (int t = 0; t < 16; t++)
            {
                w[t] |= (uint)ctx.MessageBlock[t * 4 + 0] << 24;
                w[t] |= (uint)ctx.MessageBlock[t * 4 + 1] << 16;
                w[t] |= (uint)ctx.MessageBlock[t * 4 + 2] << 08;
                w[t] |= (uint)ctx.MessageBlock[t * 4 + 3];
            }

            for (int t = 16; t < 80; t++)
                w[t] = CircularShift(1, w[t - 3] ^ w[t - 8] ^ w[t - 14] ^ w[t - 16]);

            ctx.A = ctx.IntermediateHash[0];
            ctx.B = ctx.IntermediateHash[1];
            ctx.C = ctx.IntermediateHash[2];
            ctx.D = ctx.IntermediateHash[3];
            ctx.E = ctx.IntermediateHash[4];

            for (int t = 0; t < 20; t++)
                RotateWordBuffers(ctx, (b, c, d) => b & c | ~b & d, w[t], K0);

            for (int t = 20; t < 40; t++)
                RotateWordBuffers(ctx, (b, c, d) => b ^ c ^ d, w[t], K1);

            for (int t = 40; t < 60; t++)
                RotateWordBuffers(ctx, (b, c, d) => b & c | b & d | c & d, w[t], K2);

            for (int t = 60; t < 80; t++)
                RotateWordBuffers(ctx, (b, c, d) => b ^ c ^ d, w[t], K3);

            ctx.IntermediateHash[0] += ctx.A;
            ctx.IntermediateHash[1] += ctx.B;
            ctx.IntermediateHash[2] += ctx.C;
            ctx.IntermediateHash[3] += ctx.D;
            ctx.IntermediateHash[4] += ctx.E;

            ctx.MessageBlockIndex = 0;
        }
  
        void RotateWordBuffers(Context ctx, Func<uint, uint, uint, uint> f, uint wt, uint kt)
        {
            uint temp = CircularShift(5, ctx.A) + f(ctx.B, ctx.C, ctx.D) + ctx.E + wt + kt;

            ctx.E = ctx.D;
            ctx.D = ctx.C;
            ctx.C = CircularShift(30, ctx.B);
            ctx.B = ctx.A;
            ctx.A = temp;
        }

        void PadMessage(Context ctx)
        {
            ctx.MessageBlock[ctx.MessageBlockIndex++] = 0x80;

            if (ctx.MessageBlockIndex > 55)
            {
                while (ctx.MessageBlockIndex < 64)
                    ctx.MessageBlock[ctx.MessageBlockIndex++] = 0;

                ProcessMessageBlock(ctx);
            }

            while (ctx.MessageBlockIndex < 56)
                ctx.MessageBlock[ctx.MessageBlockIndex++] = 0;

            ctx.MessageBlock[56] = (byte)(ctx.LengthHigh >> 24);
            ctx.MessageBlock[57] = (byte)(ctx.LengthHigh >> 16);
            ctx.MessageBlock[58] = (byte)(ctx.LengthHigh >> 08);
            ctx.MessageBlock[59] = (byte) ctx.LengthHigh;
            ctx.MessageBlock[60] = (byte)(ctx.LengthLow >> 24);
            ctx.MessageBlock[61] = (byte)(ctx.LengthLow >> 16);
            ctx.MessageBlock[62] = (byte)(ctx.LengthLow >> 08);
            ctx.MessageBlock[63] = (byte) ctx.LengthLow;

            ProcessMessageBlock(ctx);
        }
    }
}
