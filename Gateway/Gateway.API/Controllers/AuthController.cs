using Gateway.Abstractions;
using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ILogger<AuthController> logger, 
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp( [FromBody] SignUpModel signUpModel )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Body.");
            }

            // TODO Send user data to UserService and get user id

            int userId = 1;

            var response = await _authService.CreateTokensAsync(userId);

            AppendCookies("accessToken", response.AccessToken, DateTime.UtcNow.AddDays(3));
            AppendCookies("refreshToken", response.RefreshToken, DateTime.UtcNow.AddMonths(1));

            return Ok(response);
        }

        private void AppendCookies(string key, string value, DateTimeOffset expiresTime)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions()
            { 
                HttpOnly = true,
                Expires = expiresTime
            });
        }
    }
}
