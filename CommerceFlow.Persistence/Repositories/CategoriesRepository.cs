using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceFlow.Persistence.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly CommerceDbContext _context;
        private readonly ILogger<CategoriesRepository> _logger;

        public CategoriesRepository(
            CommerceDbContext context,
            ILogger<CategoriesRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category.Id;
        }

        public async Task<int> UpdateAsync(Category category)
        {
            var entity = await _context.Categories.FindAsync(category.Id);

            if(entity is null)
            {
                return 0;
            }

            entity.Name = category.Name;
            entity.Description = category.Description;

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if(entity is null)
            {
                return 0;
            }

            entity.Products.Clear();
            _context.Categories.Remove(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var entity = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return entity;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
