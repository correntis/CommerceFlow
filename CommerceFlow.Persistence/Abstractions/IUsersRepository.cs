using CommerceFlow.Persistence.Entities;

namespace CommerceFlow.Persistence.Abstractions
{
    public interface IUsersRepository
    {
        Task<int> AddAsync(User user);
        Task<int> DeleteAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<User> GetAsync(int id);
        Task<int> UpdateAsync(User user);
        Task<int> UpdatePasswordAsync(string email, string hashPassword);
        Task<int> UpdateRoleAsync(int id, string role);
        Task<User> GetByEmailAsync(string email);
    }
}