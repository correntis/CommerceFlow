using AuthService;
using Grpc.Core;

namespace AuthService.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceImpl> _logger;
        public AuthServiceImpl(ILogger<AuthServiceImpl> logger)
        {
            _logger = logger;
        }

        public override Task<CreateTokensResponse> CreateTokens(CreateTokensRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Method - CreateTokens - {Time}",DateTime.Now);

            var createTokensResponse = new CreateTokensResponse
            {
                AccessToken = "test",
                RefreshToken = "test"
            };

            return Task.FromResult(createTokensResponse);
        }

        public override Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Method - Verify - {Time}", DateTime.Now);

            var verifyResponse = new VerifyResponse
            {
                AccessToken = "test",
                RefreshToken = "test",
                UserId = 1,
                IsValid = true
            };

            return Task.FromResult(verifyResponse);
        }
    }
}
