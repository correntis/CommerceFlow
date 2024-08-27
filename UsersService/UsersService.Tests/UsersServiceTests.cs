using CommerceFlow.Persistence;
using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Services;

namespace UsersService.Tests
{
    public class UsersServiceTests
    {
        private readonly UsersServiceImpl _usersService;
        private readonly CommerceDbContext _context;
        private readonly IUsersRepository _usersRepository;
        private readonly Mock<ILogger<UsersServiceImpl>> _loggerUsersService;
        private readonly Mock<ILogger<UsersRepository>> _loggerUsersRepository;

        public UsersServiceTests()
        {
            _loggerUsersService = new Mock<ILogger<UsersServiceImpl>>();
            _loggerUsersRepository = new Mock<ILogger<UsersRepository>>();

            var optionsBuilder = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase("UsersServiceTestsDb");

            _context = new CommerceDbContext(optionsBuilder.Options);

            _usersRepository = new UsersRepository(_loggerUsersRepository.Object, _context);
            _usersService = new UsersServiceImpl(_loggerUsersService.Object, _usersRepository);
        }

        [Fact]
        public async Task CreateUser_ShouldSaveUserToDb()
        {
            var response = await CreateUserInMemory();


            Assert.NotNull(response);
            Assert.True(response.ResponseCase == CreateUserResponse.ResponseOneofCase.Id);
            Assert.True(response.Id > 0);
        }

        [Fact]
        public async Task UpdateExistingUser_ShouldUpdateUser()
        {
            var createResponse = await CreateUserInMemory();

            var updateRequest = new UpdateUserRequest()
            {
                Id = createResponse.Id,
                Name = "Doe Jane",
                Email = "doejogn8@gmail.com",
                HashPassword = "updated"
            };

            var updateResponse = await _usersService.Update(updateRequest, null);

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.ResponseCase == UpdateUserResponse.ResponseOneofCase.IsSuccess);
        }


        [Fact]
        public async Task UpdateNonExistingUser_ShouldReturnError()
        {
            var updateRequest = new UpdateUserRequest()
            {
                Id = int.MaxValue,
                Name = "Doe Jane",
                Email = "doejogn8@gmail.com",
                HashPassword = "updated"
            };

            var updateResponse = await _usersService.Update(updateRequest, null);

            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.ResponseCase == UpdateUserResponse.ResponseOneofCase.Error);
        }

        [Fact]
        public async Task DeleteExistingUser_ShouldDeleteUser()
        {
            var createResponse = await CreateUserInMemory();

            var deleteRequest = new DeleteUserRequest()
            {
                Id = createResponse.Id,
            };

            var deleteResponse = await _usersService.Delete(deleteRequest, null);

            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.ResponseCase == DeleteUserResponse.ResponseOneofCase.IsSuccess);
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
            Assert.True(deleteResponse.ResponseCase == DeleteUserResponse.ResponseOneofCase.Error);
        }

        [Fact]
        public async Task GetExistingUserById_ShouldReturnUser()
        {
            var createResponse = await CreateUserInMemory();

            var getRequest = new GetUserRequest()
            {
                Id = createResponse.Id
            };

            var getResponse = await _usersService.Get(getRequest, null);

            Assert.NotNull(getResponse);
            Assert.True(getResponse.ResponseCase == GetUserResponse.ResponseOneofCase.User);
            Assert.Equal(createResponse.Id, getResponse.User.Id);
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
            Assert.True(getResponse.ResponseCase == GetUserResponse.ResponseOneofCase.Error);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            var amount = 3;
            var usersId = new List<int>();

            for (var i = 0; i < amount; i++)
            {
                var createResponse =  await CreateUserInMemory();
                usersId.Add(createResponse.Id);
            }

            var response = await _usersService.GetAll(new Empty(), null);

            Assert.NotNull(response);
            Assert.Equal(amount, response.Users.Count);
            Assert.Contains(usersId, id => response.Users.Any(user => user.Id == id));
            Assert.Contains(usersId, firstId => usersId.Any(secondId => firstId > secondId));
        }

        private async Task<CreateUserResponse> CreateUserInMemory()
        {
            var request = new CreateUserRequest()
            {
                Name = "John Doe",
                Email = "johndoe78@gmail.com",
                HashPassword = "OInf13vn09NV09N493Nnnn0FJP1FK"
            };

            return await _usersService.Create(request, null);
        }
    }
}