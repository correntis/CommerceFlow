using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommerceFlow.Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ILogger<UsersRepository> _logger;
        private readonly CommerceDbContext _context;


        public UsersRepository(
            ILogger<UsersRepository> logger,
            CommerceDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<int> AddAsync(User user)
        {
            var entity = new User
            {
                Name = user.Name,
                Email = user.Email,
                HashPassword = user.HashPassword
            };

            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> UpdateAsync(User user)
        {
            var entity = await _context.Users.FindAsync(user.Id);

            if (entity is null)
            {
                return 0;
            }

            entity.Name = user.Name;
            entity.Email = user.Email;
            entity.HashPassword = user.HashPassword;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }
        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);

            if (entity is null)
            {
                return 0;
            }

            _context.Users.Remove(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<User> GetAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);

            if (entity is null)
            {
                return new();
            }

            return entity;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var entities = await _context.Users.ToListAsync();

            if (entities is null)
            {
                return [];
            }

            return entities;
        }
    }
}
