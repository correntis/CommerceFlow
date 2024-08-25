using Gateway.Abstractions;
using Grpc.Net.Client;

namespace Gateway.Services
{
    public class AuthServiceClient : IAuthService
    {
        private readonly ILogger<AuthServiceClient> _logger;
        private readonly string _address;
        public AuthServiceClient(
            ILogger<AuthServiceClient> logger, 
            IConfiguration configuration)
        {
            _logger = logger;
            _address = $"http://{configuration["AUTH_HOST"]}:{configuration["AUTH_PORT"]}";
        }

        public async Task<CreateTokensResponse> CreateTokensAsync(ulong userId)
        {
            _logger.LogInformation("Method - CreateTokens - {Time} - {Address}", DateTime.Now, _address);

            using var channel = GrpcChannel.ForAddress(_address);
            var client = new AuthService.AuthServiceClient(channel);

            var request = new CreateTokensRequest
            {
                UserId = userId
            };

            var response = await client.CreateTokensAsync(request);

            return response;
        }

        public async Task<VerifyResponse> VerifyAsync(string refreshToken)
        {
            _logger.LogInformation("Method - Verify - {Time}", DateTime.Now);

            using var channel = GrpcChannel.ForAddress(_address);
            var client = new AuthService.AuthServiceClient(channel);

            var request = new VerifyRequest
            {
                RefreshToken = refreshToken
            };

            var response = await client.VerifyAsync(request);

            return response;
        }
    }
}
