using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using CommerceFlow.Protobufs;
using UsersService.API.Abstractions;
using Grpc.Core;

namespace UsersService.API.Services
{
    public class UsersServiceImpl : CommerceFlow.Protobufs.Server.UsersService.UsersServiceBase
    {
        private readonly ILogger<UsersServiceImpl> _logger;
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsersServiceImpl(
            ILogger<UsersServiceImpl> logger,
            IUsersRepository usersRepository,
            IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
        }

        public async override Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
        {
            var user = await _usersRepository.GetByEmailAsync(request.Email);

            if(user is null)
            {
                return new AuthenticateResponse()
                {
                    IsSuccess = false
                };
            }

            if(!_passwordHasher.Verify(request.Password, user.HashPassword))
            {
                return new AuthenticateResponse()
                {
                    IsSuccess = false
                };
            }

            return new AuthenticateResponse()
            {
                IsSuccess = true,
                User = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.Name,
                    Location = new() { Address = user.Location.Address, City = user.Location.City },
                }
            };
        }

        public override async Task<CreateUserResponse> Create(CreateUserRequest request, ServerCallContext context)
        {
            if(await _usersRepository.GetByEmailAsync(request.Email) != null)
            {
                return new CreateUserResponse()
                {
                    IsSuccess = false
                };
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                HashPassword = _passwordHasher.Hash(request.Password),
                Location = new() { },
                Role = new() { Name = UserRoles.User }
            };

            var id = await _usersRepository.AddAsync(user);

            if(id == 0)
            {
                return new CreateUserResponse()
                {
                    IsSuccess = false
                };
            }

            return new CreateUserResponse() 
            { 
                IsSuccess = true, 
                Id = id 
            };
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
                Id = request.User.Id,
                Name = request.User.Name,
                Email = request.User.Email,
                Phone = request.User.Phone,
                Location = new() { Address = request.User.Location.Address, City = request.User.Location.City }
            };

            var rowsAffected = await _usersRepository.UpdateAsync(user);

            return new() { IsSuccess = Convert.ToBoolean(rowsAffected) };
        }

        public override async Task<UpdatePasswordResponse> UpdatePassword(UpdatePasswordRequest request, ServerCallContext context)
        {
            var hashPassword = _passwordHasher.Hash(request.Password);

            var rowsAffected = await _usersRepository.UpdatePasswordAsync(request.Email, hashPassword);

            return new UpdatePasswordResponse()
            {
                IsSuccess = Convert.ToBoolean(rowsAffected)
            };
        }

        public override async Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request, ServerCallContext context)
        {

            var rowsAffected = await _usersRepository.UpdateRoleAsync(request.Id, request.Role);

            return new UpdateRoleResponse()
            {
                IsSuccess = Convert.ToBoolean(rowsAffected)
            };
        }

        public override async Task<GetUserResponse> Get(GetUserRequest request, ServerCallContext context)
        {
            var user = await _usersRepository.GetAsync(request.Id);

            if(user is null)
            {
                return new GetUserResponse()
                {
                    IsSuccess = false
                };
            }

            return new GetUserResponse()
            {
                IsSuccess = true,
                User = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role.Name,
                    Location = new() { Address = user.Location.Address, City = user.Location.City }
                }
            };
        }

        public override async Task<UsersList> GetAll(Empty request, ServerCallContext context)
        {
            var users = await _usersRepository.GetAllAsync();
            var response = new UsersList();

            if(users is null)
            {
                return response;
            }

            response.Users.AddRange(users.Select(user => new UserMessage
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.Name,
                Location = new() { City = user.Location.City, Address = user.Location.Address }
            }));

            return response;
        }
    }
}
