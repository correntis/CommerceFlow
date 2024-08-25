using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Persistence
{
    public class CommerceDbContext : DbContext
    {
        public CommerceDbContext(DbContextOptions<CommerceDbContext> options)
            : base(options)
        {
            
        }
    }
}
