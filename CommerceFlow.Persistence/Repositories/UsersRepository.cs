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
                .FindAsync(user.Id);

            if (entity is null)
            {
                return 0;
            }

            await _context.Entry(entity)
                    .Reference(x => x.Location)
                    .LoadAsync();

            entity.Name = user.Name;
            entity.Email = user.Email;
            entity.Phone = user.Phone;
            entity.HashPassword = user.HashPassword;

            entity.Location.Address = user.Location.Address;
            entity.Location.City = user.Location.City;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected == 0 ? 1 : rowsAffected;
        }
        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);

            if (entity is null)
            {
                return 0;
            }

            await _context.Entry(entity)
                .Reference(x => x.Location)
                .LoadAsync();
            await _context.Entry(entity)
                .Reference(x => x.Role)
                .LoadAsync();

            _context.Users.Remove(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected == 0 ? 1 : rowsAffected;
        }

        public async Task<User> GetAsync(int id)
        {
            var entity = await _context.Users.FindAsync(id);

            if (entity is null)
            {
                return null;
            }

            await _context.Entry(entity)
                .Reference(x => x.Location)
                .LoadAsync();

            await _context.Entry(entity)
                .Reference(x => x.Role)
                .LoadAsync();

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
            var entity = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (entity is null)
            {
                return null;
            }

            await _context.Entry(entity)
                .Reference(x => x.Location)
                .LoadAsync();

            await _context.Entry(entity)
                .Reference(x => x.Role)
                .LoadAsync();

            return entity;
        }
    }
}
