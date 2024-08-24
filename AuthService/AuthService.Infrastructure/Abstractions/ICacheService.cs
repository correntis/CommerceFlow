namespace AuthService.Infrastructure.Abstractions
{
    public interface ICacheService
    {
        Task<string> GetUserIdAsync(string token);
        Task RemoveTokenAsync(string oldToken);
        Task SetTokenAsync(string token, string userId);
    }
}
