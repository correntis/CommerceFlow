using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using Grpc.Core;

namespace UsersService.Services
{
    public class UsersServiceImpl : UsersService.UsersServiceBase
    {
        private readonly ILogger<UsersServiceImpl> _logger;
        private readonly IUsersRepository _usersRepository;

        public UsersServiceImpl(
            ILogger<UsersServiceImpl> logger,
            IUsersRepository usersRepository)
        {
            _logger = logger;
            _usersRepository = usersRepository;
        }

        public override async Task<CreateUserResponse> Create(CreateUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                HashPassword = request.HashPassword
            };

            var id = await _usersRepository.AddAsync(user);

            return new() { Id = id };
        }

        public override async Task<DeleteUserResponse> Delete(DeleteUserRequest request, ServerCallContext context)
        {
            var rowsAffected = await _usersRepository.DeleteAsync(request.Id);

            return new() { IsSuccess = Convert.ToBoolean(rowsAffected) };
        }

        public override async Task<UpdateUserResponse> Update(UpdateUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Id = request.Id,
                Name = request.Name,
                Email = request.Email,
                HashPassword = request.HashPassword
            };

            var rowsAffected = await _usersRepository.UpdateAsync(user);

            return new() { IsSuccess = Convert.ToBoolean(rowsAffected) };
        }

        public override async Task<UserResponse> Get(GetUserRequest request, ServerCallContext context)
        {
            var user = await _usersRepository.GetAsync(request.Id);

            return new() { Email = user.Email, Name = user.Name, Id = user.Id };
        }

        public override async Task<UsersResponse> GetAll(Empty request, ServerCallContext context)   
        {
            var users = await _usersRepository.GetAllAsync();

            var response = new UsersResponse();

            response.Users.AddRange(users.Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsSuccess = true
            }));
            response.IsSuccess = true;

            return response;
        }
    }
}
