using CommerceFlow.Persistence.Abstractions;
using Grpc.Core;
using UsersService;

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

        public override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            return base.CreateUser(request, context);
        }

        public override Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            return base.DeleteUser(request, context);
        }

        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            return base.UpdateUser(request, context);
        }

        public override Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            return base.GetUser(request, context);
        }

        public override Task<UsersResponse> GetAllUsers(Empty request, ServerCallContext context)   
        {
            return base.GetAllUsers(request, context);   
        }

    }
}
