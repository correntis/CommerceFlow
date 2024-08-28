using CommerceFlow.Persistence;
using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using CommerceFlow.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Tests.Unit
{
    public class UsersRepositoryTests
    {
        private readonly IUsersRepository _usersRepository;
        private readonly Mock<ILogger<UsersRepository>> _loggerUsersRepository;
        private readonly CommerceDbContext _context;

        public UsersRepositoryTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase("UsersRepositoryTestsDb");

            _context = new CommerceDbContext(optionsBuilder.Options);
            _loggerUsersRepository = new Mock<ILogger<UsersRepository>>();

            _usersRepository = new UsersRepository(_loggerUsersRepository.Object, _context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserToDb()
        {
            var user = CreateUser();

            var id = await _usersRepository.AddAsync(user);
            var resultUser = await _context.Users.FindAsync(id);

            Assert.True(id > 0);
            Assert.NotNull(resultUser);
            Assert.NotNull(resultUser.Location);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var user = CreateUser();

            var id = await _usersRepository.AddAsync(user);
            var resultUser = await _context.Users.FindAsync(id);

            Assert.NotNull(resultUser);
            Assert.NotNull(resultUser.Location);


            resultUser.Name = "Othername";
            resultUser.Location.City = "Othertown";
            resultUser.Phone = "0987654321";

            await _usersRepository.UpdateAsync(resultUser);

            var updatedUser = await _context.Users.FindAsync(id);

            Assert.NotNull(updatedUser);
            Assert.Equal("Othername", updatedUser.Name);
            Assert.Equal("Othertown", updatedUser.Location.City);
            Assert.Equal("0987654321", updatedUser.Phone);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            var user = CreateUser();
            var id = await _usersRepository.AddAsync(user);
            var resultUser = await _context.Users.FindAsync(id);

            Assert.NotNull(resultUser);


            await _usersRepository.DeleteAsync(id);

            var deletedUser = await _context.Users.FindAsync(id);

            Assert.Null(deletedUser);
        }

        public User CreateUser()
        {
            return new User
            {
                Name = "John Doe",
                Email = "example@test.com",
                HashPassword = "password",
                Phone = "1234567890",
                Location = new Location
                {
                    Address = "1234 Main St",
                    City = "Anytown"
                }
            };
        }
    }
}
