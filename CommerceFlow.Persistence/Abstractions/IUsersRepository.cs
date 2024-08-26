using CommerceFlow.Persistence.Entities;

namespace CommerceFlow.Persistence.Abstractions
{
    public interface IUsersRepository
    {
        Task<ulong> AddAsync(User user);
        Task<int> DeleteAsync(ulong id);
        Task<List<User>> GetAllAsync();
        Task<User> GetAsync(ulong id);
        Task<int> UpdateAsync(User user);
    }
}