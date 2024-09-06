using AuthService.Infrastructure.Abstractions;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using CommerceFlow.Protobufs;

namespace AuthService.Services
{
    public class AuthServiceImpl : CommerceFlow.Protobufs.Server.AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceImpl> _logger;
        private readonly ITokenCacheService _cacheService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthServiceImpl(
            ILogger<AuthServiceImpl> logger,
            ITokenCacheService cacheService,
            ITokenService tokenService,
            IEmailService emailService
            )
        {
            _logger = logger;
            _cacheService = cacheService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public override async Task<CreateTokensResponse> CreateTokens(CreateTokensRequest request, ServerCallContext context)
        {
            var accessToken = IssueAccessToken(request.UserId, request.UserRole);
            var refreshToken = await IssueRefreshTokenAsync(request.UserId);

            var createTokensResponse = new CreateTokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return createTokensResponse;
        }

        public override async Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context)
        {
            var userIdString = await _cacheService.GetUserIdAsync(request.RefreshToken);

            if (userIdString.IsNullOrEmpty()) 
            {
                return new VerifyResponse
                {
                    IsSuccess = false
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
                IsSuccess = true
            };

            return verifyResponse;
        }


        public override async Task<SendPasswordResetLinkResponse> SendPasswordResetLink(SendPasswordResetLinkRequest request, ServerCallContext context)
        {
            var token = _tokenService.CreateResetPasswordToken();

            //TODO Change to frontend Reset Password Page
            //var passwordResetLink = $"http://localhost:3000/reset-password?Email={request.Email}&Token={token}";

            var isSuccess = await _emailService.SendEmailAsync(
                request.Email,
                "Reset password in CommerceFlow", 
                $"Reset your password with token {token}</a>."
            );

            await _cacheService.SetTokenAsync(token, request.Email, DateTime.UtcNow.AddMinutes(10));

            return new()
            {
                IsSuccess = isSuccess
            };
        }

        public override async Task<VerifyPasswordResetResponse> VerifyPasswordReset(VerifyPasswordResetRequest request, ServerCallContext context)
        {
            var isSuccess = await _cacheService.ContainsAsync(request.ResetToken);

            if(isSuccess)
            {
                await RemoveTokenFromCacheAsync(request.ResetToken);
            }

            return new VerifyPasswordResetResponse
            {
                IsSuccess = isSuccess
            };
        }

        public string IssueAccessToken(int userId, string userRole)
        {
            return _tokenService.CreateAccessToken(userId, userRole);
        }

        public async Task<string> IssueRefreshTokenAsync(int userId)
        {
            var refreshToken = _tokenService.CreateRefreshToken();

            await _cacheService.SetTokenAsync(refreshToken, userId.ToString(), DateTime.UtcNow.AddMonths(1));

            return refreshToken;
        }

        public async Task RemoveTokenFromCacheAsync(string oldToken)
        {
            await _cacheService.RemoveTokenAsync(oldToken);
        }
    }
}
