using Gateway.API.Contracts;
using Grpc.Net.Client;
using CSharpFunctionalExtensions;
using Gateway.API.Contracts.Authentication;
using Gateway.API.Contracts.Users;
using Gateway.API.Abstractions;
using CommerceFlow.Protobufs;
using CommerceFlow.Protobufs.Client;


namespace Gateway.API.Services
{
    public class UsersServiceClient : IUsersService
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

        public async Task<Result<UserMessage, bool>> AuthenticateAsync(LoginRequest loginRequest)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new AuthenticateRequest()
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password
            };

            var response = await usersService.AuthenticateAsync(request);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.User;
        }

        public async Task<Result<int, bool>> CreateAsync(RegisterRequest registerRequest)
        {

            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new CreateUserRequest()
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                Password = registerRequest.Password
            };

            var response = await usersService.CreateAsync(request);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.Id;
        }

        public async Task<bool> UpdateAsync(int userId, UserDto userRequest)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdateUserRequest()
            {
                User = new UserMessage()
                {
                    Id = userId,
                    Email = userRequest.Email,
                    Name = userRequest.Name,
                    Phone = userRequest.Phone,
                    Location = new UserLocationMessage()
                    {
                        City = userRequest.City,
                        Address = userRequest.Address
                    }
                }

            };

            var response = await usersService.UpdateAsync(request);

            return response.IsSuccess;
        }

        public async Task<bool> UpdatePasswordAsync(string email, string password)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdatePasswordRequest()
            {
                Email = email,
                Password = password
            };

            var response = await usersService.UpdatePasswordAsync(request);

            return response.IsSuccess;
        }

        public async Task<bool> UpdateRoleAsync(int id, string role)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new UpdateRoleRequest()
            {
                Id = id,
                Role = role
            };

            var response = await usersService.UpdateRoleAsync(request);

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

        public async Task<Result<UserMessage, bool>> GetAsync(int userId)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var request = new GetUserRequest()
            {
                Id = userId
            };

            var response = await usersService.GetAsync(request);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.User;
        }

        public async Task<UsersList> GetAllAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var usersService = new UsersService.UsersServiceClient(channel);

            var response = await usersService.GetAllAsync(new Empty());

            return response;
        }
    }
}
