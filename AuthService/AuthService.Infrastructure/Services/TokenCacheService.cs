using AuthService.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthService.Infrastructure.Services
{
    public class TokenCacheService : ITokenCacheService
    {
        private readonly IDistributedCache _cache;

        public TokenCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetTokenAsync(string token, string userId, DateTimeOffset duration)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(duration);

            await _cache.SetStringAsync(token, userId.ToString(), options);
        }

        public async Task RemoveTokenAsync(string oldToken)
        {
            await _cache.RemoveAsync(oldToken);
        }

        public async Task<string> GetUserIdAsync(string token)
        {
            var userId = await _cache.GetStringAsync(token);

            return userId;
        }

        public async Task<bool> ContainsAsync(string token)
        {
            var email = await _cache.GetStringAsync(token);

            return email != null;
        }
    }
}
