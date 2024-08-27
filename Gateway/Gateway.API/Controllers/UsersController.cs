using Gateway.API;
using Gateway.API.Models;
using Gateway.API.Services;
using Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers        
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] UpdateUserModel userModel)
        {
            var User = new User
            {
                Name = userModel.Name,
                Email = userModel.Email,
                Password = userModel.Password
            };

            var result = await _usersService.CreateAsync(User);


            return result.Match<IActionResult>(
                id => Ok($"User created with id {id}"),
                error => StatusCode(error.Code, error.Message)
            );
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel userModel)
        {
            var User = new User
            {
                Id = id,
                Name = userModel.Name,
                Email = userModel.Email,
                Password = userModel.Password
            };

            var result = await _usersService.UpdateAsync(User);

            return result.Match<IActionResult>(
                    success => Ok($"User updated"),
                    error => StatusCode(error.Code, error.Message)
            );
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _usersService.DeleteAsync(id);

            return result.Match<IActionResult>(
                    success => Ok($"User deleted"),
                    error => StatusCode(error.Code, error.Message)
            );
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _usersService.GetAsync(id);

            return result.Match<IActionResult>(
                    user => Ok(user),
                    error => StatusCode(error.Code, error.Message)
            );
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _usersService.GetAllAsync();

            return Ok(users);
        }
    }
}
