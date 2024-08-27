using Gateway.Abstractions;
using Gateway.API.Services;
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
        public async Task<IActionResult> Register( [FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Body.");
            }

            var user = new User
            {
                Name = registerModel.Name,
                Email = registerModel.Email,
                Password = registerModel.Password
            };

            var result = await _usersService.CreateAsync(user);

            if (result.IsError)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            int userId = result.Value;

            var response = await _authService.CreateTokensAsync(userId);

            AppendCookies(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login( [FromBody] LoginModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Body.");
            }

            var result = await _usersService.Authenticate(loginModel.Email, loginModel.Password);

            if (result.IsError)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            var user = result.Value;

            var response = await _authService.CreateTokensAsync(user.Id);

            AppendCookies(response);

            return Ok(user);
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
