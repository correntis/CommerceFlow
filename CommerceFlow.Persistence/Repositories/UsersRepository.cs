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
                HashPassword = user.HashPassword,
                Location = new() { },
                Role = new() { Name = user.Role.Name }
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int> UpdateAsync(User user)
        {
            var entity = await _context.Users
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (entity is null)
            {
                return 0;
            }

            entity.Name = user.Name;
            entity.Email = user.Email;
            entity.Phone = user.Phone;

            entity.Location.Address = user.Location.Address;
            entity.Location.City = user.Location.City;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<int> UpdatePasswordAsync(string email, string hashPassword)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if(entity is null)
            {
                return 0;
            }

            entity.HashPassword = hashPassword;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }


        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

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
            var entity = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return entity;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var entities = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .AsNoTracking()
                .ToListAsync();

            return entities;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Email == email);

            return entity;
        }
    }
}
