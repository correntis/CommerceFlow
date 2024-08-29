using CSharpFunctionalExtensions;
using Gateway.API.Contracts.Authentication;
using Gateway.API.Contracts.Users;

namespace Gateway.API.Abstractions
{
    public interface IUsersService
    {
        Task<Result<UserResponse, Error>> Authenticate(LoginRequest loginRequest);
        Task<Result<int, Error>> CreateAsync(RegisterRequest registerRequest);
        Task<Result<bool, Error>> DeleteAsync(int userId);
        Task<List<UserResponse>> GetAllAsync();
        Task<Result<UserResponse, Error>> GetAsync(int userId);
        Task<Result<bool, Error>> UpdateAsync(int userId, UserDto userRequest);
    }
}