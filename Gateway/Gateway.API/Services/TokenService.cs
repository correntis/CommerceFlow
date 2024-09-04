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

        public TokenService(
            IAuthService authService
            )
        {
            _authService = authService;
        }

        public async Task<bool> HandleUpdateTokenAsync(AuthenticationFailedContext context)
        {
            if(context.Request.Cookies.ContainsKey("refreshToken"))
            {
                var userRole = GetUserRoleClaim(context.Request.Cookies["accessToken"]);

                if(userRole is null)
                {
                    return false;
                }

                var refreshToken = context.Request.Cookies["refreshToken"];
                var verifyResponse = await _authService.VerifyAsync(refreshToken, userRole.Value);

                if(verifyResponse.IsValid)
                {
                    UpdateTokens(verifyResponse, context.Response);

                    return true;
                }
            }

            return false;
        }

        private Claim GetUserRoleClaim(string token)
        {
            var oldTokenDecoded = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            var userRole = oldTokenDecoded.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role);

            return userRole;
        }

        private void UpdateTokens(VerifyResponse verifyResponse, HttpResponse response)
        {
            response.Cookies.Append("accessToken", verifyResponse.AccessToken,
                new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
            );

            response.Cookies.Append("refreshToken", verifyResponse.RefreshToken,
                new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddMonths(1) }
            );
        }
    }
}
