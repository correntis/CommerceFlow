using CSharpFunctionalExtensions;
using Gateway.API;
using Gateway.API.Contracts.Users;
using Gateway.API.Contracts;
using Gateway.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gateway.API.Abstractions;
using Gateway.API.Infrastructure;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _usersService;

        public UsersController(
            ILogger<UsersController> logger,
            IUsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usersService.UpdateAsync(id, userDto);

            if (result.IsFailure)
            {
                return StatusCode(result.Error.Code, result.Error.Message);
            }

            return Ok($"User updated");
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateUserRole(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok($"User role updated" + id);
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
