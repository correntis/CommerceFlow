namespace Gateway.Abstractions
{
    public interface IAuthService
    {
        Task<CreateTokensResponse> CreateTokensAsync(int userId, string userRole);
        Task<VerifyResponse> VerifyAsync(string refreshToken, string userRole);
        Task<SendPasswordResetLinkResponse> SendResetPasswordLink(string email);

    }
}