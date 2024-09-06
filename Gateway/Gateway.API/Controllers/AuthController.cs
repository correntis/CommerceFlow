using Gateway.Abstractions;
using Gateway.API.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;
using Gateway.API.Infrastructure;
using Gateway.API.Abstractions;
using CommerceFlow.Protobufs;


namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsersService _usersService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthService authService,
            IUsersService usersService)
        {
            _logger = logger;
            _authService = authService;
            _usersService = usersService;
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
                return StatusCode(StatusCodes.Status400BadRequest, "Incorrect credentials");
            }

            var response = await _authService.CreateTokensAsync(createResult.Value, UserRoles.User);

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

            var authResult = await _usersService.AuthenticateAsync(request);

            if (authResult.IsFailure)
            {
                return StatusCode(404, "User Not Found");
            }

            var response = await _authService.CreateTokensAsync(authResult.Value.Id, authResult.Value.Role);

            AppendCookies(response);

            return Ok(authResult.Value);
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _authService.SendResetPasswordLink(request.Email);

            if (!response.IsSuccess)
            {
                return StatusCode(500, "Internal Server Error");
            }
            return Ok();
        }

        [HttpPatch("password")]
        public async Task<IActionResult> ResetPassword([FromQuery] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(); 
            }

            var response = await _authService.VerifyPasswordReset(request.Token);

            if(!response.IsSuccess)
            {
                return NotFound();
            }

            var updateResponse = await _usersService.UpdatePasswordAsync(request.Email, request.Password);

            return Ok();
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
