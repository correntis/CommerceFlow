namespace AuthService.Infrastructure.Abstractions
{
    public interface ITokenService
    {
        string CreateAccessToken(int userId, string userRole);
        string CreateRefreshToken();
    }
}