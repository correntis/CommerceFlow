using Gateway.API.Contracts;
using Grpc.Net.Client;
using CSharpFunctionalExtensions;
using Gateway.API.Contracts.Authentication;
using Gateway.API.Contracts.Users;

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

        public async Task<Result<UserResponse, Error>> Authenticate(LoginRequest loginRequest)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new AuthenticateRequest()
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password
            };

            var response = await usersService.AuthenticateAsync(request);

            if (response.ResponseCase == AuthenticateResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            return response.User;
        }

        public async Task<Result<int, Error>> CreateAsync(RegisterRequest registerRequest)
        {

            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new CreateUserRequest()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Password = registerRequest.Password,
                Location = new UserLocation()
            };

            var response = await usersService.CreateAsync(request);

            if (response.ResponseCase == CreateUserResponse.ResponseOneofCase.Error)
            {
                return response.Error;
            }

            return response.Id;
        }

        public async Task<Result<bool,Error>> UpdateAsync(int userId, UserDto userRequest)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdateUserRequest()
            {
                Id = userId,
                Email = userRequest.Email,
                Name = userRequest.Name,
                Password = userRequest.Password,
                Phone = userRequest.Phone,
                Location = new UserLocation()
                {
                    City = userRequest.City,
                    Address = userRequest.Address
                }

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

        public async Task<Result<UserResponse, Error>> GetAsync(int userId)
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

            return response.User;
        }

        public async Task<List<UserResponse>> GetAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var response = await usersService.GetAllAsync(new Empty());

            return [.. response.Users];
        }
    }
}
