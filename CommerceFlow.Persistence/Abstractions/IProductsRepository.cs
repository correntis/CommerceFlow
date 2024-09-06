using CommerceFlow.Persistence.Entities;

namespace CommerceFlow.Persistence.Abstractions
{
    public interface IProductsRepository
    {
        Task<int> AddAsync(Product product);
        Task<int> DeleteAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<int> UpdateAsync(Product product);
    }
}