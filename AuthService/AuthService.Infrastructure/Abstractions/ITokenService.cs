namespace AuthService.Infrastructure.Abstractions
{
    public interface ITokenService
    {
        string CreateAccessToken(ulong userId);
        string CreateRefreshToken();
    }
}