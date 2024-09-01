using AuthService.Infrastructure.Configuration;
using AuthService.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenService(
            IOptions<JwtOptions> jwtOptions
            )
        {
            _jwtOptions = jwtOptions;
        }

        public string CreateAccessToken(int userId, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtClaims = new List<Claim>()
            {
                new("User_Id", userId.ToString()),
                new(ClaimTypes.Role, userRole),
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string CreateResetPasswordToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
