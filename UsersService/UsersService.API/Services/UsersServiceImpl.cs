using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using Grpc.Core;
using UsersService.API.Abstractions;

namespace UsersService.Services
{
    public class UsersServiceImpl : UsersService.UsersServiceBase
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

            if (user is null)
            {
                return new AuthenticateResponse()
                {
                    Error = new() { Code = StatusCodes.Status404NotFound, Message = "User Not Found" }
                };
            }

            if (!_passwordHasher.Verify(request.Password, user.HashPassword))
            {
                return new AuthenticateResponse()
                {
                    Error = new() { Code = StatusCodes.Status401Unauthorized, Message = "Unauthorized" }
                };
            }

            return new AuthenticateResponse() 
            {
                User = new() 
                { 
                    Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role.Name,
                    Location = new() { Address = user.Location.Address, City = user.Location.City },
                }
            };
        }

        public override async Task<CreateUserResponse> Create(CreateUserRequest request, ServerCallContext context)
        {
            if (await _usersRepository.GetByEmailAsync(request.Email) != null)
            {
                return new CreateUserResponse()
                {
                    Error = new()
                    {
                        Code = StatusCodes.Status409Conflict, Message = "User Already Exists"
                    }
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
                Phone = request.Phone,
                HashPassword = _passwordHasher.Hash(request.Password),
                Location = new() { Address = request?.Location.Address, City = request?.Location.City }
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

        public override async Task<UpdatePasswordResponse> UpdatePassword(UpdatePasswordRequest request, ServerCallContext context)
        {
            var hashPassword = _passwordHasher.Hash(request.Password);

            var rowsAffected = await _usersRepository.UpdatePasswordAsync(request.Email, hashPassword);

            return new UpdatePasswordResponse()
            {
                IsSuccess = Convert.ToBoolean(rowsAffected)
            };
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
                    Phone = user.Phone,
                    Role = user.Role.Name,
                    Location = new() { Address = user.Location.Address, City = user.Location.City }
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
                Phone = user.Phone,
                Role = user.Role.Name,
                Location = new() { City = user.Location.City, Address = user.Location.Address }
            }));

            return response;
        }
    }
}
