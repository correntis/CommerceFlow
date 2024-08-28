using CSharpFunctionalExtensions;
using Gateway.API;
using Gateway.API.Contracts.Users;
using Gateway.API.Contracts;
using Gateway.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UsersServiceClient _usersService;

        public UsersController(
            ILogger<UsersController> logger,
            UsersServiceClient usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var result = await _usersService.UpdateAsync(id, userDto);

            if (result.IsFailure)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            return Ok($"User updated");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _usersService.DeleteAsync(id);

            if (result.IsFailure)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            return Ok($"User deleted");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _usersService.GetAsync(id);

            if (result.IsFailure)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            return Ok(result.Value);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _usersService.GetAllAsync();

            return Ok(users);
        }
    }
}
