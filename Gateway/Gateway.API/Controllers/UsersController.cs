using Gateway.API.Contracts.Users;
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
        public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isSuccess = await _usersService.UpdateAsync(id, userDto);

            if (!isSuccess)
            {
                return StatusCode(404, "User Not Found");
            }

            return Ok($"User updated");
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            var isSuccess = await _usersService.UpdateRoleAsync(id, role);

            if(isSuccess)
            {
                return Ok($"User role updated");
            }

            return StatusCode(404, "User Not Found");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _usersService.DeleteAsync(id);

            if (isSuccess)
            {
                return Ok($"User deleted");
            }

            return StatusCode(404, "User Not Found");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _usersService.GetAsync(id);

            if (result.IsFailure)
            {
                return StatusCode(404, "User Not Found");
            }

            return Ok(result.Value);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _usersService.GetAllAsync();

            return Ok(users);
        }
    }
}
