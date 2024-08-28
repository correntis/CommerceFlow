using Gateway.Abstractions;
using Gateway.API.Contracts.Authentication;
using Gateway.API.Contracts;
using Gateway.API.Services;
using Microsoft.AspNetCore.Mvc;
using Gateway.API.Contracts.Users;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UsersServiceClient _usersService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthService authService,
            UsersServiceClient _usersService)
        {
            _logger = logger;
            _authService = authService;
            this._usersService = _usersService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createResult = await _usersService.CreateAsync(request);

            if (createResult.IsFailure)
            {
                return StatusCode(createResult.Error.Code, createResult.Error.Message);
            }

            var response = await _authService.CreateTokensAsync(createResult.Value);

            AppendCookies(response);

            return Ok(createResult.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResult = await _usersService.Authenticate(request);

            if (authResult.IsFailure)
            {
                return StatusCode(authResult.Error.Code, authResult.Error.Message);
            }

            var response = await _authService.CreateTokensAsync(authResult.Value.Id);

            AppendCookies(response);

            return Ok(authResult.Value);
        }

        private void AppendCookies(CreateTokensResponse response)
        {
            HttpContext.Response.Cookies.Append("accessToken", response.AccessToken, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMonths(1)
            });
            HttpContext.Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMonths(1)
            });
        }
    }
}
