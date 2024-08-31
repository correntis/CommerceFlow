namespace AuthService.Infrastructure.Abstractions
{
    public interface ITokenCacheService
    {
        Task<string> GetUserIdAsync(string token);
        Task RemoveTokenAsync(string oldToken);
        Task SetTokenAsync(string token, string userId, DateTimeOffset duration);
        Task<bool> ContainsAsync(string token);
    }
}
