using Gateway.Models;
using Grpc.Net.Client;

namespace Gateway.API.Services
{
    public class UsersServiceClient
    {
        private readonly ILogger<UsersServiceClient> _logger;

        private readonly string address;

        public UsersServiceClient(
            ILogger<UsersServiceClient> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            address = $"http://{configuration["USERS_HOST"]}:{configuration["USERS_PORT"]}";
        }

        public async Task<int> CreateAsync(User user)
        {

            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new CreateUserRequest()
            {
                Email = user.Email,
                Name = user.Name,
                HashPassword = user.HashPassword
            };

            var response = await usersService.CreateAsync(request);

            return response.Id;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdateUserRequest()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                HashPassword = user.HashPassword
            };

            var response = await usersService.UpdateAsync(request);

            return response.IsSuccess;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new DeleteUserRequest()
            {
                Id = userId
            };

            var response = await usersService.DeleteAsync(request);

            return response.IsSuccess;
        }

        public async Task<User> GetAsync(int userId)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new GetUserRequest()
            {
                Id = userId
            };

            var response = await usersService.GetAsync(request);

            var user = new User()
            {
                Id = response.Id,
                Name = response.Name,
                Email = response.Email
            };

            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var response = await usersService.GetAllAsync(new Empty());

            var users = response.Users.Select(User => new User()
            {
                Id = User.Id,
                Name = User.Name,
                Email = User.Email

            });

            return users.ToList();
        }
    }
}
