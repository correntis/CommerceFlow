using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
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

        public ulong Add(User user)
        {
            return 1;
        }

        public void Update(User user)
        {

        }
        public void Delete(ulong id)
        {

        }

        public User Get(ulong id)
        {
            return new User();
        }

        public List<User> GetAll()
        {
            return [];
        }
    }
}
