using Gateway.Models;
using Gateway.Services;
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

        [HttpPost("login/{id}")]
        public async Task<IActionResult> Login(ulong id)
        {
            _logger.LogInformation("POST login/{id}", id);
            await _authService.CreateTokens(id);
            return Ok();

        }
    }
}
