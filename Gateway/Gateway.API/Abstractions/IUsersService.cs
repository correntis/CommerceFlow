using CSharpFunctionalExtensions;
using Gateway.API.Contracts.Authentication;
using Gateway.API.Contracts.Users;
using CommerceFlow.Protobufs;

namespace Gateway.API.Abstractions
{
    public interface IUsersService
    {
        Task<Result<UserMessage, bool>> AuthenticateAsync(LoginRequest loginRequest);
        Task<Result<int, bool>> CreateAsync(RegisterRequest registerRequest);
        Task<bool> DeleteAsync(int userId);
        Task<UsersList> GetAllAsync();
        Task<Result<UserMessage, bool>> GetAsync(int userId);
        Task<bool> UpdateAsync(int userId, UserDto userRequest);
        Task<bool> UpdatePasswordAsync(string email, string password);
        Task<bool> UpdateRoleAsync(int id, string role);
    }
}