using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public interface IOAuth2Authorizer : IAuthorizer
    {
        Task BeginAuthorizeAsync(string? state);
        Task CompleteAuthorizeAsync(string code, string? state);
        Task<string> RevokeTokenAsync();
        Task<string> RefreshTokenAsync();
    }
}
