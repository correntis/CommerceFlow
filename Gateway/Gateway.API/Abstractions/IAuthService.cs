namespace Gateway.Abstractions
{
    public interface IAuthService
    {
        Task<CreateTokensResponse> CreateTokensAsync(ulong userId);
        Task<VerifyResponse> VerifyAsync(string refreshToken);
    }
}