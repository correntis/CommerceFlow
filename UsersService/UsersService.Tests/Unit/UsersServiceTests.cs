using CommerceFlow.Persistence;
using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using CommerceFlow.Persistence.Repositories;
using CommerceFlow.Protobufs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.API.Abstractions;
using UsersService.API.Infrastructure;
using UsersService.API.Services;

namespace UsersService.Tests.Unit
{
    public class UsersServiceTests
    {
        private readonly UsersServiceImpl _usersService;
        private readonly CommerceDbContext _context;
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly Mock<ILogger<UsersServiceImpl>> _loggerUsersService;
        private readonly Mock<ILogger<UsersRepository>> _loggerUsersRepository;

        public UsersServiceTests()
        {
            _loggerUsersService = new Mock<ILogger<UsersServiceImpl>>();
            _loggerUsersRepository = new Mock<ILogger<UsersRepository>>();

            var optionsBuilder = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase("UsersServiceTestsDb");

            _context = new CommerceDbContext(optionsBuilder.Options);
            _passwordHasher = new PasswordHasher();

            _usersRepository = new UsersRepository(_loggerUsersRepository.Object, _context);
            _usersService = new UsersServiceImpl(_loggerUsersService.Object, _usersRepository, _passwordHasher);
        }

        [Fact]
        public async Task CreateUser_ShouldSaveUserToDb()
        {
            var user = CreateUser(0);
            var response = await CreateUserInMemoryAsync(user);

            var getRequest = new GetUserRequest()
            {
                Id = response.Id
            };

            var resultUser = await _usersService.Get(getRequest, null);

            Assert.NotNull(response);
            Assert.NotNull(resultUser);
            Assert.Equal(UserRoles.User, resultUser.User.Role);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task UpdateExistingUser_ShouldUpdateUser()
        {
            var user = CreateUser(1);
            var createResponse = CreateUserInMemoryAsync(user);

            var updateRequest = new UpdateUserRequest()
            {
                User = new UserMessage
                {
                    Id = createResponse.Id,
                    Name = "Doe Jane",
                    Email = "doejogn8@gmail.com",
                    Phone = "0987654321",
                    Location = new UserLocationMessage()
                    {
                        Address = "4321 Elm St",
                        City = "Othertown"
                    }
                }
            };

            var updateResponse = await _usersService.Update(updateRequest, null);
            var resultUser = await _context.Users.FindAsync(createResponse.Id);

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.IsSuccess);
        }

        [Fact]
        public async Task UpdateUserPassword_ShouldUpdatePassword()
        {
            var user = CreateUser(2);
            var createResponse = await CreateUserInMemoryAsync(user);

            var updatePasswordRequest = new UpdatePasswordRequest()
            {
                Email = user.Email,
                Password = "test"
            };

            var updateResponse = await _usersService.UpdatePassword(updatePasswordRequest, null);
            var authResponse = await _usersService.Authenticate(new AuthenticateRequest()
            {
                Email = user.Email,
                Password = updatePasswordRequest.Password
            }, null);

            Assert.NotNull(authResponse);
            Assert.True(authResponse.IsSuccess);
        }

            [Fact]
        public async Task UpdateNonExistingUser_ShouldReturnError()
        {
            var updateRequest = new UpdateUserRequest()
            {
                User = new UserMessage()
                {
                    Id = int.MaxValue,
                    Name = "Doe Jane",
                    Email = "doejogn8@gmail.com",
                    Location = new UserLocationMessage()
                }
            };

            var updateResponse = await _usersService.Update(updateRequest, null);

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.IsSuccess);
        }

        [Fact]
        public async Task DeleteExistingUser_ShouldDeleteUser()
        {
            var user = CreateUser(3);
            var createResponse = await CreateUserInMemoryAsync(user);

            var deleteRequest = new DeleteUserRequest()
            {
                Id = createResponse.Id,
            };

            var deleteResponse = await _usersService.Delete(deleteRequest, null);

            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.IsSuccess);
        }

        [Fact]
        public async Task DeleteNonExistingUser_ShouldReturnError()
        {
            var deleteRequest = new DeleteUserRequest()
            {
                Id = int.MaxValue,
            };

            var deleteResponse = await _usersService.Delete(deleteRequest, null);

            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.IsSuccess);
        }

        [Fact]
        public async Task GetExistingUserById_ShouldReturnUser()
        {
            var user = CreateUser(4);
            var createResponse = await CreateUserInMemoryAsync(user);

            var getRequest = new GetUserRequest()
            {
                Id = createResponse.Id
            };

            var getResponse = await _usersService.Get(getRequest, null);

            Assert.NotNull(getResponse);
            Assert.True(getResponse.IsSuccess);
            Assert.Equal(createResponse.Id, getResponse.User.Id);
            Assert.Equal(user.Name, getResponse.User.Name);
        }

        [Fact]
        public async Task GetNonExistingUserById_ShouldReturnError()
        {
            var getRequest = new GetUserRequest()
            {
                Id = int.MaxValue
            };

            var getResponse = await _usersService.Get(getRequest, null);

            Assert.NotNull(getResponse);
            Assert.True(getResponse.IsSuccess);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            var amount = 3;
            var usersId = new List<int>();

            for (var i = 5; i < amount; i++)
            {
                var user = CreateUser(i);
                var createResponse = await CreateUserInMemoryAsync(user);
                usersId.Add(createResponse.Id);
            }

            var response = await _usersService.GetAll(new Empty(), null);

            Assert.NotNull(response);
        }

        private User CreateUser()
        {
            return new User
            {
                Name = "John Doe",
                Email = "example@test.com",
                HashPassword = "password"
            };
        }

        private User CreateUser(int i)
        {
            return new User
            {
                Name = "John Doe" + i,
                Email = "example@test.com" + i,
                HashPassword = "password" + i
            };
        }

        private async Task<CreateUserResponse> CreateUserInMemoryAsync(User user)
        {
            var request = new CreateUserRequest()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.HashPassword,

            };

            return await _usersService.Create(request, null);
        }
    }
}