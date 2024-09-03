using CommerceFlow.Persistence.Entities;

namespace CommerceFlow.Persistence.Abstractions
{
    public interface ICategoriesRepository
    {
        Task<int> AddAsync(Category category);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task<int> UpdateAsync(Category category);
    }
}