using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public interface IOAuth2Authorizer : IAuthorizer
    {
        Task BeginAuthorizeAsync();
        Task CompleteAuthorizeAsync(string code);
        Task LogoutAsync();
        Task RefreshAsync();
    }
}
