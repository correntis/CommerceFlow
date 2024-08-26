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

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] UpdateUserModel userModel)
        {
            var User = new User
            {
                Name = userModel.Name,
                Email = userModel.Email,
                HashPassword = userModel.Password
            };

            var id = await _usersService.CreateAsync(User);

            return Ok($"User created with id {id}");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _usersService.GetAsync(id);

            return Ok(user);
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
