namespace Gateway.Abstractions
{
    public interface IAuthService
    {
        Task<CreateTokensResponse> CreateTokensAsync(int userId);
        Task<VerifyResponse> VerifyAsync(string refreshToken);
    }
}