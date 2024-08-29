using AuthService.Infrastructure.Abstractions;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceImpl> _logger;
        private readonly ITokenCacheService _cacheService;
        private readonly ITokenService _tokenService;

        public AuthServiceImpl(
            ILogger<AuthServiceImpl> logger,
            ITokenCacheService cacheService,
            ITokenService tokenService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _tokenService = tokenService;
        }

        public override async Task<CreateTokensResponse> CreateTokens(CreateTokensRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Create access and refresh tokens...");

            var accessToken = IssueAccessToken(request.UserId, request.UserRole);
            var refreshToken = await IssueRefreshTokenAsync(request.UserId);

            var createTokensResponse = new CreateTokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            _logger.LogInformation("Tokens created...");
            return createTokensResponse;
        }

        public override async Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Verify refresh token...");

            var userIdString = await _cacheService.GetUserIdAsync(request.RefreshToken);

            if (userIdString.IsNullOrEmpty()) 
            {
                _logger.LogInformation("Refresh token doesn't exist in RedisCache. Return false");

                return new VerifyResponse
                {
                    IsValid = false
                };
            }

            await RemoveTokenFromCacheAsync(request.RefreshToken);

            var userId = int.Parse(userIdString);

            var newAccessToken = IssueAccessToken(userId, request.UserRole);
            var newRefreshToken = await IssueRefreshTokenAsync(userId);
            
            var verifyResponse = new VerifyResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = userId,
                IsValid = true
            };

            return verifyResponse;
        }

        public string IssueAccessToken(int userId, string userRole)
        {
            return _tokenService.CreateAccessToken(userId, userRole);
        }

        public async Task<string> IssueRefreshTokenAsync(int userId)
        {
            _logger.LogInformation("Issue Refresh Token");

            var refreshToken = _tokenService.CreateRefreshToken();

            await _cacheService.SetTokenAsync(refreshToken, userId.ToString());

            return refreshToken;
        }

        public async Task RemoveTokenFromCacheAsync(string oldToken)
        {
            await _cacheService.RemoveTokenAsync(oldToken);
        }
    }
}
