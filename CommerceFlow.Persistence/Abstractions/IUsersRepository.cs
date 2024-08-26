using CommerceFlow.Persistence.Entities;

namespace CommerceFlow.Persistence.Abstractions
{
    public interface IUsersRepository
    {
        ulong Add(User user);
        void Delete(ulong id);
        User Get(ulong id);
        List<User> GetAll();
        void Update(User user);
    }
}