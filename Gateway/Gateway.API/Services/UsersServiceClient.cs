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

        public async Task<Result<User, Error>> Authenticate(string email, string password)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new AuthenticateRequest()
            {
                Email = email,
                Password = password
            };

            var response = await usersService.AuthenticateAsync(request);

            if (response.ResponseCase == AuthenticateResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            var user = new User()
            {
                Id = response.User.Id,
                Name = response.User.Name,
                Email = response.User.Email
            };

            return user;
        }

        public async Task<Result<int, Error>> CreateAsync(User user)
        {

            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new CreateUserRequest()
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password
            };

            var response = await usersService.CreateAsync(request);

            if (response.ResponseCase == CreateUserResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            return response.Id;
        }

        public async Task<Result<bool,Error>> UpdateAsync(User user)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdateUserRequest()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Password = user.Password
            };

            var response = await usersService.UpdateAsync(request);

            if (response.ResponseCase == UpdateUserResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            return response.IsSuccess;
        }

        public async Task<Result<bool,Error>> DeleteAsync(int userId)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new DeleteUserRequest()
            {
                Id = userId
            };

            var response = await usersService.DeleteAsync(request);

            if (response.ResponseCase == DeleteUserResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            return response.IsSuccess;
        }

        public async Task<Result<User, Error>> GetAsync(int userId)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new GetUserRequest()
            {
                Id = userId
            };

            var response = await usersService.GetAsync(request);

            if (response.ResponseCase == GetUserResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            var user = new User()
            {
                Id = response.User.Id,
                Name = response.User.Name,
                Email = response.User.Email
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
