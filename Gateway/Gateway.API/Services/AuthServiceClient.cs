using Gateway.Abstractions;
using Gateway.API.Infrastructure;
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
            _logger.LogInformation("Method - CreateTokens - {Time} - {Address}", DateTime.Now, address);

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
            _logger.LogInformation("Method - Verify - {Time}", DateTime.Now);

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
    }
}
