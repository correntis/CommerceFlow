using Gateway.Abstractions;
using CommerceFlow.Protobufs.Client;
using CommerceFlow.Protobufs;
using Grpc.Net.Client;

namespace Gateway.API.Services
{
    public class AuthServiceClient : IAuthService
    {
        private readonly ILogger<AuthServiceClient> _logger;
        private readonly string address;
        public AuthServiceClient(
            ILogger<AuthServiceClient> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            address = $"http://{configuration["AUTH_HOST"]}:{configuration["AUTH_PORT"]}";
        }

        public async Task<CreateTokensResponse> CreateTokensAsync(int userId, string userRole)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new AuthService.AuthServiceClient(channel);
          
            var request = new CreateTokensRequest
            {
                UserId = userId,
                UserRole = userRole
            };

            var response = await client.CreateTokensAsync(request);

            return response;
        }

        public async Task<VerifyResponse> VerifyAsync(string refreshToken, string userRole)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new AuthService.AuthServiceClient(channel);

            var request = new VerifyRequest
            {
                RefreshToken = refreshToken,
                UserRole = userRole
            };

            var response = await client.VerifyAsync(request);

            return response;
        }

        public async Task<SendPasswordResetLinkResponse> SendResetPasswordLink(string email)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new AuthService.AuthServiceClient(channel);

            var request = new SendPasswordResetLinkRequest
            {
                Email = email
            };

            var response = await client.SendPasswordResetLinkAsync(request);

            return response;
        }

        public async Task<VerifyPasswordResetResponse> VerifyPasswordReset(string token)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new AuthService.AuthServiceClient(channel);

            var request = new VerifyPasswordResetRequest
            {
                Token = token
            };

            var response = await client.VerifyPasswordResetAsync(request);

            return response;
        }
    }
}
