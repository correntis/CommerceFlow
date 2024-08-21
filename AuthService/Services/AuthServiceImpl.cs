using AuthService;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthServiceImpl> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public AuthServiceImpl(ILogger<AuthServiceImpl> logger, IConfiguration configuration, IDistributedCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
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

            var userId = ulong.Parse(userIdString);

            RemoveRefreshTokenFromCache(request.RefreshToken);
            var newAccessToken = IssueAccessToken(userId);
            var newRefreshToken = await IssueRefreshTokenAsync(userId);
            
            var verifyResponse = new VerifyResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = userId,
                IsValid = true
            };

            _logger.LogInformation("Return new tokens");

            return verifyResponse;
        }

        private string IssueAccessToken(ulong userId)
        {
            _logger.LogInformation("Issue Access Token");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtClaims = new List<Claim>()
            {
                new("User_Id", userId.ToString()),
                new(JwtRegisteredClaimNames.Sub, userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: jwtClaims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials                
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> IssueRefreshTokenAsync(ulong userId)
        {
            _logger.LogInformation("Issue Refresh Token");

            var refreshToken = Guid.NewGuid().ToString();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddMonths(1));

            await _cache.SetStringAsync(refreshToken, userId.ToString(), options);

            return refreshToken;
        }

        private void RemoveRefreshTokenFromCache(string oldToken)
        {
            _logger.LogInformation("Remove old refresh token from RedisCache");

            _cache.Remove(oldToken);
        }
    }
}
