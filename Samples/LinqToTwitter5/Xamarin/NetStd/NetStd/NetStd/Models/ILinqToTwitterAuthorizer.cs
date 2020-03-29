using LinqToTwitter;

namespace NetStd.Models
{
    public interface ILinqToTwitterAuthorizer
    {
        IAuthorizer GetAuthorizer(string consumerKey, string consumerSecret);
    }
}
