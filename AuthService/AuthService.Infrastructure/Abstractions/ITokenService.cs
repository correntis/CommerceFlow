namespace AuthService.Infrastructure.Abstractions
{
    public interface ITokenService
    {
        string CreateAccessToken(int userId);
        string CreateRefreshToken();
    }
}