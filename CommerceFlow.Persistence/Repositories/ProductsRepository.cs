using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Configuration;
using CommerceFlow.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceFlow.Persistence.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly CommerceDbContext _context;
        private readonly ILogger<CommerceDbContext> _logger;

        public ProductsRepository(
            CommerceDbContext context,
            ILogger<CommerceDbContext> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AddAsync(Product product)
        {
            var categories = new List<Category>();

            foreach (var category in product.Categories)
            {
                categories.Add(await _context.Categories.FindAsync(category.Id));    
            }

            product.Categories.Clear();
            product.Categories.AddRange(categories);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        public async Task<int> UpdateAsync(Product product)
        {
            var entity = await _context.Products.FindAsync(product.Id);

            if(entity is null)
            {
                return 0;
            }

            entity.Name = product.Name;
            entity.Description = product.Description;
            entity.Price = product.Price;
            entity.Stock = product.Stock;

            var categories = new List<Category>();
            foreach(var category in product.Categories)
            {
                categories.Add(await _context.Categories.FindAsync(category.Id));
            }

            product.Categories.Clear();
            product.Categories.AddRange(categories);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(entity is null)
            {
                return 0;
            }

            entity.Categories.Clear();
            _context.Products.Remove(entity);

            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Categories)
                .ToListAsync();
        }
    }
}
