using Gateway.Abstractions;
using Gateway.API.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CommerceFlow.Protobufs;

namespace Gateway.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IAuthService _authService;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IAuthService authService,
            ILogger<TokenService> _logger
            )
        {
            _authService = authService;
            this._logger = _logger;
        }

        public async Task<string> HandleUpdateTokenAsync(HttpContext context)
        {
            if(context.Request.Cookies.ContainsKey("refreshToken"))
            {
                var userRole = GetUserRoleClaim(context.Request.Cookies["accessToken"]);

                if(userRole is null)
                {
                    return null;
                }

                var refreshToken = context.Request.Cookies["refreshToken"];
                var verifyResponse = await _authService.VerifyAsync(refreshToken, userRole.Value);

                if(verifyResponse.IsSuccess)
                {
                    UpdateTokens(verifyResponse, context);
                    return verifyResponse.AccessToken;
                }
            }

            return null;
        }

        private Claim GetUserRoleClaim(string token)
        {
            var oldTokenDecoded = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            var userRole = oldTokenDecoded.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role);

            return userRole;
        }

        private void UpdateTokens(VerifyResponse verifyResponse, HttpContext context)
        {
            context.Response.Cookies.Append("accessToken", verifyResponse.AccessToken,
                new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
            );

            context.Response.Cookies.Append("refreshToken", verifyResponse.RefreshToken,
                new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
            );
        }
    }
}
