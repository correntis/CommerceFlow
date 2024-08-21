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
        private readonly AuthServiceClient _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, AuthServiceClient authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Start proccessing SingUp request");

                // TODO Send user data to UserService and get user id

                ulong userId = 1;

                var response = await _authService.CreateTokensAsync(userId);

                _logger.LogInformation("Return response with tokens");
                return Ok(response);
            }
            return BadRequest("Invalid Request Body.");
        }

        [HttpGet("user/{id}")]
        [Authorize]
        public IActionResult GetUser(int id)
        {
            _logger.LogInformation("GET user/{id}", id);
            return Ok();
        }
    }
}
