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

            if (id == 0)
            {
                return new CreateUserResponse()
                {
                    Error = new() 
                    { 
                        Code = StatusCodes.Status500InternalServerError, Message = "Internal Server Error" 
                    }
                };
            }

            return new CreateUserResponse() { Id = id };
        }

        public override async Task<DeleteUserResponse> Delete(DeleteUserRequest request, ServerCallContext context)
        {
            var rowsAffected = await _usersRepository.DeleteAsync(request.Id);

            if (rowsAffected == 0)
            {
                return new DeleteUserResponse()
                {
                    Error = new() { Code = StatusCodes.Status404NotFound, Message = "User Not Found" }
                };
            }

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

            if (rowsAffected == 0)
            {
                return new UpdateUserResponse()
                {
                    Error = new() { Code = StatusCodes.Status404NotFound, Message = "User Not Found" }
                };
            }

            return new() { IsSuccess = Convert.ToBoolean(rowsAffected) };
        }

        public override async Task<GetUserResponse> Get(GetUserRequest request, ServerCallContext context)
        {
            var user = await _usersRepository.GetAsync(request.Id);

            if (user is null)
            {
                return new GetUserResponse()
                {
                    Error = new() { Code = StatusCodes.Status404NotFound, Message = "User Not Found" }
                };
            }

            return new GetUserResponse() 
            { 
                User = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                }
            };
        }

        public override async Task<UsersResponse> GetAll(Empty request, ServerCallContext context)   
        {
            var users = await _usersRepository.GetAllAsync();
            var response = new UsersResponse();

            if (users is null)
            {
                return response;
            }

            response.Users.AddRange(users.Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            }));

            return response;
        }
    }
}
