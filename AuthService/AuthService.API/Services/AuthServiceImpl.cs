using AuthService;
using AuthService.Infrascructure;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceImpl> _logger;
        private readonly IDistributedCache _cache;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public AuthServiceImpl(
            ILogger<AuthServiceImpl> logger, 
            IDistributedCache cache, 
            IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _cache = cache;
            _jwtOptions = jwtOptions;
        }

        public override async Task<CreateTokensResponse> CreateTokens(CreateTokensRequest request, ServerCallContext context)
        {

            _logger.LogInformation("Create access and refresh tokens...");

            var accessToken = IssueAccessToken(request.UserId);
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

            var userIdString = await _cache.GetStringAsync(request.RefreshToken);

            if (userIdString.IsNullOrEmpty()) 
            {
                _logger.LogInformation("Refresh token doesn't exist in RedisCache. Return false");

                return new VerifyResponse
                {
                    IsValid = false
                };
            }

            await RemoveRefreshTokenFromCacheAsync(request.RefreshToken);

            var userId = ulong.Parse(userIdString);
            var newAccessToken = IssueAccessToken(userId);
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

        public string IssueAccessToken(ulong userId)
        {
            _logger.LogInformation("Issue Access Token");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtClaims = new List<Claim>()
            {
                new("User_Id", userId.ToString()),
                new(JwtRegisteredClaimNames.Sub, userId.ToString())
            };

            var expiresTime = DateTime.UtcNow.AddDays(3);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: jwtClaims,
                expires: expiresTime,
                signingCredentials: credentials                
                );

            _logger.LogInformation("Access token expires at {Time}", expiresTime);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> IssueRefreshTokenAsync(ulong userId)
        {
            _logger.LogInformation("Issue Refresh Token");

            var refreshToken = Guid.NewGuid().ToString();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddMonths(1));

            await _cache.SetStringAsync(refreshToken, userId.ToString(), options);

            return refreshToken;
        }

        public async Task RemoveRefreshTokenFromCacheAsync(string oldToken)
        {
            await _cache.RemoveAsync(oldToken);
        }
    }
}
