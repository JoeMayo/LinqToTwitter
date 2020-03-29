namespace LinqToTwitter.Security
{
    public interface IHash
    {
        byte[] Compute(byte[] key);
    }
}